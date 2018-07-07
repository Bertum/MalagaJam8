using UnityEngine;

public class Character : MonoBehaviour
{
    public bool teleported;
    private float health;
    public bool isRight;
    private JoystickController joystickController;
    public ConstantForce constForce;

    private void Awake()
    {
        teleported = false;
        constForce = GetComponent<ConstantForce>();
        this.joystickController = GetComponent<JoystickController>();
    }

    // Use this for initialization
    void Start () {
    
    }

    // Update is called once per frame
    void Update () {

	}

    private void FixedUpdate()
    {
        //if (this.joystickController.IsPressButtonY())
        //{
        //    changeMyGravity();
        //}

        if (this.joystickController.IsPressButtonX())
        {
            invertGravity();
        }
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

    public void changeMyGravity()
    {
        if (constForce.force.y != 0)
        {
            constForce.force = new Vector3(0, 0, 0);
        }
        else {
            if (Physics.gravity.y > 0) { 
                constForce.force = new Vector3(0, -18.0f, 0);
            }
            else if (Physics.gravity.y < 0)
            {
                constForce.force = new Vector3(0, 18.0f, 0);
            }
        }
        
    }

    public void invertGravity () {
        Physics.gravity = new Vector3(0, Physics.gravity.y * -1);
        if (constForce.force.y != 0) { constForce.force = new Vector3(0, constForce.force.y * -1, 0); }
    }

    public void DropWeapon(bool value)
    {
        throw new System.NotImplementedException();
    }

    public void Shoot(bool value)
    {
        throw new System.NotImplementedException();
    }

}
