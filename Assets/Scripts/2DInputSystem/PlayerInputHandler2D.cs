using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInputHandler2D : MonoBehaviour
{
    [Header("Character Input Values")]
    public Vector2 move;
    public bool jump;
    public bool sprint;

    private PlayerInputAction inputActions;

    private void Awake()
    {
        inputActions = new PlayerInputAction();
    }

    private void OnEnable()
    {
        inputActions.Player.Enable();

        // Move: Menggunakan performed dan canceled agar pergerakan responsif
        inputActions.Player.Move.performed += OnMove;
        inputActions.Player.Move.canceled += OnMove;

        // Jump: Menggunakan started untuk deteksi tekan dan canceled untuk lepas
        inputActions.Player.Jump.started += OnJump;
        inputActions.Player.Jump.canceled += OnJump;

        // Sprint
        inputActions.Player.Sprint.performed += OnSprint;
        inputActions.Player.Sprint.canceled += OnSprint;
    }

    private void OnDisable()
    {
        inputActions.Player.Move.performed -= OnMove;
        inputActions.Player.Move.canceled -= OnMove;

        inputActions.Player.Jump.started -= OnJump;
        inputActions.Player.Jump.canceled -= OnJump;

        inputActions.Player.Sprint.performed -= OnSprint;
        inputActions.Player.Sprint.canceled -= OnSprint;

        inputActions.Player.Disable();
    }

    #region Input Action Callbacks
    private void OnMove(InputAction.CallbackContext ctx)
    {
        move = ctx.ReadValue<Vector2>();
    }

    private void OnJump(InputAction.CallbackContext ctx)    
    {   
        jump = ctx.ReadValueAsButton(); 
    }   

    private void OnSprint(InputAction.CallbackContext ctx)  
    {   
        sprint = ctx.ReadValueAsButton();   
    }   
    #endregion
}