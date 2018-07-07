using UnityEngine;

public class ScreenShake : MonoBehaviour
{
    private Vector3 originalPos;
    public float shakeAmount = 0.7F;
    public float decreaseFactor = 1.0F;
    public float shakeDuration = 1;

    public static ScreenShake Instance { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            originalPos = this.transform.position;
            Instance = this;
        }
    }

    private void Update()
    {
        if (shakeDuration > 0)
        {
            this.transform.localPosition = originalPos + Random.insideUnitSphere * shakeAmount;

            shakeDuration -= Time.deltaTime * decreaseFactor;
        }
        else
        {
            shakeDuration = 0f;
            this.transform.localPosition = originalPos;
        }
    }


}
