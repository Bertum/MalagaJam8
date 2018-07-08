using UnityEngine;

public class Character : MonoBehaviour
{
    private static readonly string TAG_ITEM_INVERT_GLOBAL_GRAVITY = "ItemInvertGlobalGravity";

    public bool teleported;
    private float health;
    public bool isRight;
    public Transform floorObject;
    public LayerMask floorMask;

    private JoystickController joystickController;
    private ConstantForce2D constantForceComponent;
    private float floorRadius = 0.07f;
    private bool touchingFloor;

    private void Awake()
    {
        teleported = false;
        constantForceComponent = GetComponent<ConstantForce2D>();
        this.joystickController = GetComponent<JoystickController>();
    }

    // Use this for initialization
    void Start()
    {
        this.touchingFloor = true;
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void FixedUpdate()
    {
        if (this.joystickController.IsPressButtonX())
        {
            this.joystickController.SetInvertGravity(!this.joystickController.GetInvertGravity());
            changeMyGravity();
        }


        //Comprobar si el personaje esta tocando el suelo/un bloque.
        this.joystickController.CanJump = Physics2D.OverlapCircle(this.floorObject.position, this.floorRadius, this.floorMask);
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
        if (constantForceComponent.force.y != 0)
        {
            constantForceComponent.force = new Vector2(0, 0);
        }
        else
        {
            if (Physics2D.gravity.y > 0)
            {
                constantForceComponent.force = new Vector2(0, -18.0f);
            }
            else if (Physics2D.gravity.y < 0)
            {
                constantForceComponent.force = new Vector2(0, 18.0f);
            }
        }

    }

    public void DropWeapon(bool value)
    {
        throw new System.NotImplementedException();
    }

    public void Shoot(bool value)
    {
        throw new System.NotImplementedException();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag.Equals(TAG_ITEM_INVERT_GLOBAL_GRAVITY))
        {
            GameController.Instance.InvertGlobalGravity();
        }
    }

    public JoystickController GetJoystickController()
    {
        return this.joystickController;
    }

}
