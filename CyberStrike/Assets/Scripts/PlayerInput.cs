using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInput : MonoBehaviour
{
    public enum ButtonState
    {
        Started,
        Performed,
        Canceled
    }
    private CyberStrikeInputAction inputAction;

    public Vector2 moveInput { get; private set; }
    public Vector2 mouseMoveInput { get; private set; }
    public bool fire { get; private set; }
    public bool jump
    {
        get
        {
            return inputAction.PlayerActions.Jump.triggered;
        }
    }
    public bool sprint { get; private set; }
    public bool reload { get; private set; }
    public bool zoom { get; private set; }

    private void Start()
    {
        inputAction = new CyberStrikeInputAction();
        inputAction.PlayerActions.Enable();
        inputAction.PlayerActions.Move.performed += MoveControl;
        inputAction.PlayerActions.Move.canceled += context => moveInput = Vector2.zero;

        inputAction.PlayerActions.MouseMove.performed += OnMouseMove;
        inputAction.PlayerActions.MouseMove.canceled += context => mouseMoveInput = Vector2.zero;

        //inputAction.PlayerActions.Jump.started += context =>
        //{
        //    jump = OnButtonDown(context);
        //};
        //inputAction.PlayerActions.Jump.performed += context =>
        //{
        //    jump = OnButtonDown(context);
        //};
        //inputAction.PlayerActions.Jump.canceled += context => jump = false;

        inputAction.PlayerActions.Fire.performed += context => fire = true;
        inputAction.PlayerActions.Fire.canceled += context => fire = false;

        inputAction.PlayerActions.Sprint.performed += context => sprint = true;
        inputAction.PlayerActions.Sprint.canceled += context => sprint = false;

        inputAction.PlayerActions.Reload.performed += context => reload = true;
        inputAction.PlayerActions.Reload.canceled += context => reload = false;

        inputAction.PlayerActions.Zoom.performed += context => zoom = true;
        inputAction.PlayerActions.Zoom.canceled += context => zoom = false;
    }
    private void MoveControl(InputAction.CallbackContext context)
    {
        moveInput = context.ReadValue<Vector2>();
        if (moveInput.sqrMagnitude > 1) moveInput = moveInput.normalized;
    }
    //private bool OnButtonDown(InputAction.CallbackContext context)
    //{
    //    return context.phase == InputActionPhase.Started;
    //}
    private void OnMouseMove(InputAction.CallbackContext context)
    {
        mouseMoveInput = context.ReadValue<Vector2>();
    }
}
