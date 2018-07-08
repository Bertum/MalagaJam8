using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JoystickController : MonoBehaviour
{
    private static readonly string JOYSTICK_LEFT_ONE = "JL1";//Joystick izquierdo - horizontal
    private static readonly string JOYSTICK_A = "AB";//Joystick A button
    private static readonly string JOYSTICK_B = "BB";//Joystick B button
    private static readonly string JOYSTICK_X = "XB";//Joystick X button
    private static readonly string JOYSTICK_Y = "YB";//Joystick Y button
    //private static readonly string JOYSTICK_RIGHT_ONE_PLATER_ONE = "JR1";//Joystick derecho - horizontal
    //private static readonly string JOYSTICK_RIGHT_TWO_PLATER_ONE = "JR2";//Joystick derecho - vertical

    public float maxHorizontalSpeed = 10f;
    public float jumpForce = 300f;
    public bool invertHorizontalMovement = false;
    public int numberPlayer = 1;

    private bool invertGravity = false;
    private Rigidbody2D rigidbodyComponent;
    public bool CanJump
    {
        get; set;
    }

    private void Start()
    {
        this.CanJump = true;
    }

    private void Awake()
    {
        this.rigidbodyComponent = GetComponent<Rigidbody2D>();
    }

    private void FixedUpdate()
    {
        Move(GetValueJoystickLeft());
        Jump(GetButtonDown(JOYSTICK_A));
    }

    private void Jump(bool value)
    {
        if (value && this.CanJump)
        {
            float y = this.jumpForce * (this.invertGravity ? -1 : 1);
            this.rigidbodyComponent.AddForce(new Vector2(0f, y));
        }
    }

    private void Move(float horizontalMovement)
    {
        if (this.invertHorizontalMovement)
        {
            horizontalMovement = horizontalMovement * (-1);
        }

        this.rigidbodyComponent.velocity = new Vector2(horizontalMovement * this.maxHorizontalSpeed, this.rigidbodyComponent.velocity.y);
    }

    private float GetAxis(string axisName)
    {
        return Input.GetAxis(axisName + GetNumberPlayer());
    }

    private bool GetButtonDown(string buttonName)
    {
        return Input.GetButtonDown(buttonName + GetNumberPlayer());
    }

    private string GetNumberPlayer()
    {
        return this.tag + this.numberPlayer.ToString();
    }

    public void SetInvertHorizontalMovement(bool value)
    {
        this.invertHorizontalMovement = value;
    }

    public void SetJumpForce(float value)
    {
        this.jumpForce = value;
    }

    public bool IsJoystickLeftHorizontalLeft()
    {
        return GetAxis(JOYSTICK_LEFT_ONE) < 0f;
    }

    public bool IsJoystickLeftHorizontalRight()
    {
        return GetAxis(JOYSTICK_LEFT_ONE) > 0f;
    }

    public float GetValueJoystickLeft()
    {
        return GetAxis(JOYSTICK_LEFT_ONE);
    }

    public bool IsPressButtonA()
    {
        return GetButtonDown(JOYSTICK_A);
    }

    public bool IsPressButtonB()
    {
        return GetButtonDown(JOYSTICK_B);
    }

    public bool IsPressButtonX()
    {
        return GetButtonDown(JOYSTICK_X);
    }

    public bool IsPressButtonY()
    {
        return GetButtonDown(JOYSTICK_Y);
    }

    public void SetInvertGravity(bool value)
    {
        this.invertGravity = value;
    }
    public bool GetInvertGravity()
    {
        return this.invertGravity;
    }

}
