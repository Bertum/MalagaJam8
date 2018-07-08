using UnityEngine;

public class WoodController : MonoBehaviour
{
    public bool canMove = true;
    private int randomSpeed;

    private void Awake()
    {
        randomSpeed = Random.Range(6, 11);
    }

    private void Start()
    {
        Destroy(this.gameObject, 10);
    }

    void Update()
    {
        if (canMove)
        {
            this.transform.Translate(Vector3.left * randomSpeed * Time.deltaTime);
        }
    }
}
