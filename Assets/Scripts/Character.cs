using UnityEngine;

public class Character : MonoBehaviour
{
    private static readonly string TAG_ITEM_INVERT_GLOBAL_GRAVITY = "ItemInvertGlobalGravity";

    public bool teleported;
    public float health;
    public bool isRight;
    public Transform floorCheck;
    public Transform weaponPosition;
    public LayerMask floorMask;

    private JoystickController joystickController;
    private ConstantForce2D constantForceComponent;
    private float floorRadius = 0.07f;
    private bool hasWeapon;
    private GameObject weaponTriggerObject;
    private bool canGetWeapon;
    public float playerScale = 0.5f;
    private float moveValue;
    public float generalGravity = 15;

    private void Awake()
    {
        constantForceComponent = GetComponent<ConstantForce2D>();
        this.joystickController = GetComponent<JoystickController>();
    }

    // Use this for initialization
    void Start()
    {
        teleported = false;
        this.canGetWeapon = false;
        this.hasWeapon = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (this.joystickController.IsPressButtonY())
        {
            if (this.canGetWeapon && !this.hasWeapon)
            {
                GetWeapon();
            }
            else if (this.hasWeapon)
            {
                DropWeapon();
            }
        }
    }

    private void FixedUpdate()
    {
        moveValue = joystickController.GetValueJoystickLeft();
        if (moveValue > 0)
        {
            transform.localScale = new Vector3(playerScale, transform.localScale.y);
            this.isRight = true;
        }
        else if (moveValue < 0)
        {
            transform.localScale = new Vector3(playerScale * -1, transform.localScale.y);
            this.isRight = false;
        }

        if (this.joystickController.IsPressButtonX())
        {
            this.joystickController.SetInvertGravity(!this.joystickController.GetInvertGravity());
            changeMyGravity();
        }


        //Comprobar si el personaje esta tocando el suelo.
        this.joystickController.CanJump = Physics2D.OverlapCircle(this.floorCheck.position, this.floorRadius, this.floorMask);
    }

    private void GetWeapon()
    {
        if (this.hasWeapon || !checkWeaponTriggerObject())
        {
            return;
        }

        this.weaponTriggerObject.GetComponent<WeaponController>().SetOwner(this);
        this.hasWeapon = true;
    }

    public void changeMyGravity()
    {
        if (constantForceComponent.force.y != 0)
        {
            constantForceComponent.force = new Vector2(0, 0);
            transform.localScale = new Vector3(playerScale, transform.localScale.y * -1);
        }
        else
        {
            if (Physics2D.gravity.y > 0)
            {
                constantForceComponent.force = new Vector2(0, generalGravity * -2);
                transform.localScale = new Vector3(playerScale, transform.localScale.y * -1);
            }
            else if (Physics2D.gravity.y < 0)
            {
                constantForceComponent.force = new Vector2(0, generalGravity * 2);
                transform.localScale = new Vector3(playerScale, transform.localScale.y * -1);
            }
        }

    }

    public void DropWeapon()
    {
        if (this.hasWeapon && checkWeaponTriggerObject())
        {
            this.weaponTriggerObject.GetComponent<WeaponController>().SetOwner(null);
            this.hasWeapon = false;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag.Equals(TAG_ITEM_INVERT_GLOBAL_GRAVITY))
        {
            GameController.Instance.InvertGlobalGravity();
        }

        var weaponController = collision.gameObject.GetComponent<WeaponController>();
        var bulletController = collision.gameObject.GetComponent<BulletController>();

        if (collision.gameObject.tag == "Bullet" && bulletController.character != this.gameObject.GetComponent<Character>())
        {
            health -= bulletController.damage;
        }
        else if (weaponController != null)
        {
            if (collision.gameObject.tag == "MeleeWeapon" && weaponController.attacking)
            {
                health -= weaponController.damage;
            }
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (!this.hasWeapon && IsWeaponCollider(collision) && !weaponHasOwner(collision) && !WeaponEqualTrigger(collision))
        {
            this.weaponTriggerObject = collision.gameObject;
            this.canGetWeapon = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        this.canGetWeapon = false;

        if (!this.hasWeapon)
        {
            this.weaponTriggerObject = null;
        }
    }

    public JoystickController GetJoystickController()
    {
        return this.joystickController;
    }

    public Transform GetWeaponPosition()
    {
        return this.weaponPosition;
    }

    private bool weaponHasOwner(Collider2D collision)
    {
        if (collision == null || collision.gameObject.GetComponent<WeaponController>() == null)
        {
            return false;
        }

        return collision.gameObject.GetComponent<WeaponController>().hasOwner();
    }

    private bool IsWeaponCollider(Collider2D collision)
    {
        return collision.gameObject.tag.Equals("MeleeWeapon") || collision.gameObject.tag.Equals("Weapon");
    }

    private bool WeaponEqualTrigger(Collider2D collision)
    {
        return collision.gameObject.Equals(this.weaponTriggerObject);
    }

    private bool checkWeaponTriggerObject()
    {
        return this.weaponTriggerObject != null && this.weaponTriggerObject.GetComponent<WeaponController>() != null;
    }

}
