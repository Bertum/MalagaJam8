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
    private static readonly string JOYSTICK_RIGHT_ONE_PLATER_ONE = "JR1";//Joystick derecho - horizontal
    private static readonly string JOYSTICK_RIGHT_TWO_PLATER_ONE = "JR2";//Joystick derecho - vertical

    public float maxHorizontalSpeed = 10f;
    public float jumpForce = 500f;
    public bool invertHorizontalMovement = false;
    public bool invertGlobalGravity = false;

    private Rigidbody2D rigidbody;

    private void Awake()
    {
        this.rigidbody = GetComponent<Rigidbody2D>();
    }

    private void FixedUpdate()
    {
        Move(GetValueJoystickLeft());
        Jump(GetButtonDown(JOYSTICK_A));
    }

    private void Jump(bool value)
    {
        if (value)
        {
            float y = this.jumpForce * (this.invertGlobalGravity ? (-1) : 1);
            this.rigidbody.AddForce(new Vector3(0f, y));
        }
    }

    private void Move(float horizontalMovement)
    {
        if (this.invertHorizontalMovement)
        {
            horizontalMovement = horizontalMovement * (-1);
        }

        this.rigidbody.velocity = new Vector2(horizontalMovement * this.maxHorizontalSpeed, this.rigidbody.velocity.y);
    }

    private float GetAxis(string axisName)
    {
        return Input.GetAxis(axisName + this.tag);
    }

    private bool GetButtonDown(string buttonName)
    {
        return Input.GetButtonDown(buttonName + this.tag);
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
        this.invertGlobalGravity = value;
    }
}
