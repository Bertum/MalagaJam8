using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameSceneManager : MonoBehaviour
{
    public string InitialScene;
    public float TransitionSpeed;
    public float TransitionTarget;

    private TransitionAnimationManager _animationManager;
    private TransitionSceneManager _sceneManager;
    private TransitionState _state;
    
	// Use this for initialization
	public void Start ()
    {
        _state = TransitionState.Loading;
        _sceneManager = new TransitionSceneManager(
            SceneManager.GetSceneByName(InitialScene),
            // Playable scenes - lobby - parent scene
            SceneManager.sceneCountInBuildSettings,
            // Origin will be the inverse one of target
            -(TransitionTarget));
        _animationManager = new TransitionAnimationManager(TransitionTarget, TransitionSpeed);
	}
	
	// Update is called once per frame
	public void Update ()
    {
        switch (_state)
        {
            case TransitionState.Activating:
                // Level finished, activate next level
                _state = TransitionState.Idle;
                StartCoroutine(_sceneManager.Activate(() =>
                {
                    _state = TransitionState.Activated;
                    _animationManager.AddTransition(_sceneManager.Current, _sceneManager.Next);
                }));
                break;
            case TransitionState.Activated:
                // Activate executed, start transition animation
                if (_animationManager.Update())
                    _state = TransitionState.Unloading;
                break;
            case TransitionState.Unloading:
                // Animation finished, start unloading old level
                _state = TransitionState.Idle;
                StartCoroutine(_sceneManager.Unload(() => _state = TransitionState.Loading));
                break;
            case TransitionState.Loading:
                // Unloading successful, load next level in background
                StartCoroutine(_sceneManager.PreLoad());
                _state = TransitionState.Idle;
                break;
            default:
                // Loading finished/Operation in progress, continue normally...
                break;
        }
    }

    /// <summary>
    /// Activates the transition to the next level
    /// </summary>
    public void NextLevel()
    {
        _state = TransitionState.Activating;
    }
}

public class TransitionAnimationManager
{
    private Dictionary<GameObject, float> _toMove;
    private HashSet<GameObject> _toRemove;
    private readonly float _target;
    private readonly float _speed;

    public TransitionAnimationManager(float target, float speed)
    {
        _target = target;
        _speed = speed;

        _toMove = new Dictionary<GameObject, float>();
        _toRemove = new HashSet<GameObject>();
    }

    public void AddTransition(params Scene[] scenes)
    {
        // Add all root game objects to perform the transition
        foreach (var item in scenes.Where(s => s.IsValid()).SelectMany(s => s.GetRootGameObjects()))
            _toMove.Add(item, item.transform.position.x + _target);
    }

    // Called in every update
    public bool Update()
    {
        foreach (var kvp in _toMove)
        {
            var item = kvp.Key;
            var target = kvp.Value;

            // Interpolate
            var @new = Mathf.Lerp(item.transform.position.x, target, _speed * Time.deltaTime);
            if (Mathf.Abs(@new - target) < 0.1f)
                _toRemove.Add(item);

            // Update position
            var position = item.transform.position;
            position.x = @new;
            item.transform.position = position;
        }

        // Clear down
        foreach (var item in _toRemove)
            _toMove.Remove(item);
        _toRemove.Clear();

        return _toMove.Count == 0;
    }
}

public class TransitionSceneManager
{
    public Scene Current { get; private set; }
    public Scene Next => _next ?? default(Scene);

    private Scene? _next;
    private readonly int _scenes;
    private readonly float _origin;

    private AsyncOperation _preLoadOp;

    public TransitionSceneManager(Scene initial, int nScenes, float origin)
    {
        Current = initial;
        _next = null;
        _scenes = nScenes;
        _origin = origin;
    }

    public IEnumerator PreLoad()
    {
        // Get the next index (or first if we are at the end)
        int nextIdx = Current.buildIndex + 1;
        if (nextIdx > _scenes - 1)
            nextIdx = 2;

        // Start loading without activate the new scene
        _preLoadOp = SceneManager.LoadSceneAsync(nextIdx, LoadSceneMode.Additive);
        _preLoadOp.allowSceneActivation = false;

        // Scene loading takes 90% and the remaining 10% awakes the scene when activation is enabled
        while (_preLoadOp.progress < 0.89f) yield return null;

        _next = SceneManager.GetSceneByBuildIndex(nextIdx);
    }

    public IEnumerator Activate(Action onFinished = null)
    {
        // If preload hasn't been executed, abort
        if (_preLoadOp == null) yield break;

        // Enable scene activation
        _preLoadOp.allowSceneActivation = true;

        // Wait until it's finished
        while (!_preLoadOp.isDone) yield return null;

        // Move origin
        SetupOrigin();

        // Invoke callback, if there's any
        onFinished?.Invoke();
    }

    public IEnumerator Unload(Action onFinished = null)
    {
        // Swap next scene with current one
        var old = Current;
        Current = _next.Value;

        // Unload old scene
        var unload = SceneManager.UnloadSceneAsync(old);

        // Wait until unload is complete
        while (!unload.isDone) yield return null;

        // Invoke callback if there's any
        onFinished?.Invoke();
    }

    private void SetupOrigin()
    {
        // Ensure the next scene is loaded
        if (_next == null) return;

        foreach (var item in _next.Value.GetRootGameObjects())
        {
            var position = item.transform.position;
            position.x += _origin;
            item.transform.position = position;
        }
    }
}

public enum TransitionState
{
    Activating,
    Activated,
    Unloading,
    Loading,
    Idle
}