using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class NewPlayerController : MonoBehaviour
{
    NewControls input;
    [SerializeField] float speed;
    Vector2 moveInput;
    private void Start()
    {
        input = new NewControls();
        input.PlayerInput.Enable();
        input.PlayerInput.Move.performed += MoveControl;
        input.PlayerInput.Move.canceled += context=>moveInput=Vector2.zero;
    }
    private void Update()
    {
        print(moveInput);
        var moveDirection = moveInput * Time.deltaTime * speed;
        transform.Translate(moveDirection.x, 0f, moveDirection.y);
    }
    //private void OnMove(InputValue value)
    //{
    //    moveInput = value.Get<Vector2>();
    //}
    public void MoveControl(InputAction.CallbackContext context)
    {
        moveInput = context.ReadValue<Vector2>();
    }
}
