using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInput : MonoBehaviour
{
    string fireButtonName = "Fire1";
    string jumpButtonName = "Jump";
    string moveHorizontalAxisName = "Horizontal";
    string moveVerticalAxisName = "Vertical";
    
    public Vector2 moveInput { get; private set; }
    public bool fire { get; private set; }
    public bool jump { get; private set; }

    private void Update()
    {
        moveInput = new Vector2(Input.GetAxisRaw(moveHorizontalAxisName), Input.GetAxisRaw(moveVerticalAxisName));
        if (moveInput.sqrMagnitude > 1) moveInput = moveInput.normalized;
        jump = Input.GetButtonDown(jumpButtonName);
        fire = Input.GetButton(fireButtonName);
    }
}
