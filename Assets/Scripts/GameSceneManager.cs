using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameSceneManager : MonoBehaviour
{
    public string InitialScene;
    public float TransitionTime;
    public float TransitionTarget;
    public Character[] Characters;

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
        _animationManager = new TransitionAnimationManager(TransitionTarget, TransitionTime, Characters);

        if (TransitionTime == 0)
            TransitionTime = 1;
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
    private Dictionary<GameObject, KeyValuePair<Vector2, Vector2>> _toMove;
    private HashSet<GameObject> _toRemove;
    private float lerpPercentage = 0f;

    private readonly float _target;
    private readonly float _time;
    private readonly Character[] _characters;

    public TransitionAnimationManager(float target, float time, Character[] characters)
    {
        _target = target;
        _time = time;
        _characters = characters;

        _toMove = new Dictionary<GameObject, KeyValuePair<Vector2, Vector2>>();
        _toRemove = new HashSet<GameObject>();
    }

    public void AddTransition(Scene current, Scene next)
    {
        lerpPercentage = 0f;

        // Add all root game objects to perform the transition
        var rgos = current.GetRootGameObjects().Concat(next.GetRootGameObjects());
        foreach (var item in rgos)
        {
            var position = item.transform.position;
            _toMove.Add(item, new KeyValuePair<Vector2, Vector2>(position, new Vector2(position.x + _target, position.y)));
        }

        // Add characters interpolations
        var spawns = GameObject.FindGameObjectsWithTag("Respawn").Where(o => o.scene.name == next.name).ToList();
        foreach (var character in _characters)
        {
            var i = UnityEngine.Random.Range(0, spawns.Count);
            var spawn = spawns[i];
            spawns.RemoveAt(i);

            _toMove.Add(character.gameObject, new KeyValuePair<Vector2, Vector2>(character.transform.position,
                new Vector2(spawn.transform.position.x + _target, spawn.transform.position.y)));

            //character.DropWeapon(true);
            EnableDisableCharacter(false, character);
        }
    }

    // Called in every update
    public bool Update()
    {
        lerpPercentage += Time.deltaTime / _time;
        foreach (var kvp in _toMove)
        {
            var item = kvp.Key;
            var source = kvp.Value.Key;
            var target = kvp.Value.Value;

            // Interpolate
            var @new = Vector2.Lerp(source, target, lerpPercentage);
            if (Vector2.SqrMagnitude(@new - target) < 0.001f)
                _toRemove.Add(item);

            // Update position
            item.transform.position = @new;
        }

        // Clear down
        foreach (var item in _toRemove)
            _toMove.Remove(item);

        _toRemove.Clear();

        var res = _toMove.Count == 0;
        if (res)
        {
            foreach (var character in _characters)
                EnableDisableCharacter(true, character);
        }

        return res;
    }

    private void EnableDisableCharacter(bool enable, Character character)
    {
        character.GetComponent<Rigidbody2D>().isKinematic = !enable;
        character.GetComponent<Rigidbody2D>().gravityScale = enable ? 1 : 0;
        character.GetComponent<Collider2D>().enabled = enable;
        character.GetComponent<JoystickController>().enabled = enable;
        character.enabled = enable;
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