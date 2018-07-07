using UnityEngine;

public class BulletController : MonoBehaviour
{
    private bool move = false;
    public float damage = 0;
    private float speed = 6;
    private int number;
    private float dispersionInt = 0.1f;
    private Vector3 direction;
    private SpriteRenderer spriteRenderer;
    private Sprite inversionSprite;
    private Sprite bazookaSprite;

    private void Awake()
    {
        move = false;
        spriteRenderer = GetComponent<SpriteRenderer>();
        inversionSprite = Resources.Load<Sprite>("Sprites/Bullets/inversionBullet");
        bazookaSprite = Resources.Load<Sprite>("Sprites/Bullets/misile"); ;
    }

    public void ActivateAndMove(Vector3 weaponPosition, int number, bool right, WeaponType weaponType)
    {
        if (weaponType != 0)
        {
            ChangeSprite(weaponType);
        }
        direction = right ? Vector3.right : Vector3.left;
        this.transform.position = weaponPosition;
        this.gameObject.SetActive(true);
        move = true;
        this.number = number;
        Destroy(this.gameObject, 5);
    }

    private void ChangeSprite(WeaponType weaponType)
    {
        switch (weaponType)
        {
            case WeaponType.Bazooka:
                spriteRenderer.sprite = bazookaSprite;
                break;
            case WeaponType.Inversion:
                spriteRenderer.sprite = inversionSprite;
                break;
        }
    }

    private void Update()
    {
        if (move)
        {
            switch (number)
            {
                case 0:
                    this.transform.Translate(direction * Time.deltaTime * speed);
                    break;
                case 1:
                    this.transform.Translate(Vector3.up * dispersionInt * Time.deltaTime * speed);
                    this.transform.Translate(direction * Time.deltaTime * speed);
                    break;
                case 2:
                    this.transform.Translate(Vector3.down * dispersionInt * Time.deltaTime * speed);
                    this.transform.Translate(direction * Time.deltaTime * speed);
                    break;
            }

        }
    }
}
