using UnityEngine;

public class BulletController : MonoBehaviour
{
    private bool move = false;
    public float damage = 0;
    private float speed = 6;
    private int number;
    private float dispersionInt = 0.1f;
    private Vector3 direction;

    private void Awake()
    {
        move = false;
    }

    public void ActivateAndMove(Vector3 weaponPosition, int number, bool right)
    {
        direction = right ? Vector3.right : Vector3.left;
        this.transform.position = weaponPosition;
        this.gameObject.SetActive(true);
        move = true;
        this.number = number;
        Destroy(this.gameObject, 5);
    }

    private void Update()
    {
        if (move)
        {
            switch (number)
            {
                case 0:
                    this.transform.Translate(Vector3.right * Time.deltaTime * speed);
                    break;
                case 1:
                    this.transform.Translate(Vector3.up * dispersionInt * Time.deltaTime * speed);
                    this.transform.Translate(Vector3.right * Time.deltaTime * speed);
                    break;
                case 2:
                    this.transform.Translate(Vector3.down * dispersionInt * Time.deltaTime * speed);
                    this.transform.Translate(Vector3.right * Time.deltaTime * speed);
                    break;
            }

        }
    }
}
