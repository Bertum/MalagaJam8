using UnityEngine;

public class Character : MonoBehaviour
{
    public bool teleported;
    private float health;
    public bool isRight;

    private void Awake()
    {
        teleported = false;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        var weaponController = collision.gameObject.GetComponent<WeaponController>();
        if (collision.gameObject.tag == "Bullet")
        {
            health -= collision.gameObject.GetComponent<BulletController>().damage;
        }
        else if (collision.gameObject.tag == "MeleeWeapon" && weaponController.attacking)
        {
            health -= weaponController.damage;
        }
    }

    private void GetWeapon()
    {
        //Call weaponController set owner
    }

    private void DropWeapon()
    {

    }
}
