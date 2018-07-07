using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour
{
    public static GameController Instance
    {
        get; private set;
    }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            Instance = this;
        }
    }

    public void InvertGlobalGravity()
    {
        Physics2D.gravity = new Vector2(0, Physics2D.gravity.y * -1);
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");

        foreach (GameObject player in players)
        {
            JoystickController joystickController = player.GetComponent<JoystickController>();
            joystickController.SetInvertGravity(!joystickController.GetInvertGravity());
            ConstantForce2D constantForce = player.GetComponent<ConstantForce2D>();

            if (constantForce.force.y != 0)
            {
                constantForce.force = new Vector2(0, constantForce.force.y * -1);
            }
        }
    }
}
