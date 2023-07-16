using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInput : MonoBehaviour
{
    string fireButtonName = "Fire1";
    string jumpButtonName = "Jump";
    string sprintButtonName = "Sprint";
    string zoomButtonName = "Zoom";
    string moveHorizontalAxisName = "Horizontal";
    string moveVerticalAxisName = "Vertical";
    string mouseHorizontalAxisName = "Mouse X";
    string mouseVerticalAxisName = "Mouse Y";

    public Vector2 moveInput { get; private set; }
    public Vector2 mouseInput { get; private set; }
    public bool fire { get; private set; }
    public bool jump { get; private set; }
    public bool sprint { get; private set; }
    public bool zoom { get; private set; }

    private void Update()
    {
        moveInput = new Vector2(Input.GetAxisRaw(moveHorizontalAxisName), Input.GetAxisRaw(moveVerticalAxisName));
        if (moveInput.sqrMagnitude > 1) moveInput = moveInput.normalized;
        mouseInput = new Vector2(Input.GetAxis(mouseHorizontalAxisName), Input.GetAxis(mouseVerticalAxisName));
        jump = Input.GetButtonDown(jumpButtonName);
        fire = Input.GetButton(fireButtonName);
        sprint = Input.GetButton(sprintButtonName);
        zoom = Input.GetButton(zoomButtonName);
    }
}
