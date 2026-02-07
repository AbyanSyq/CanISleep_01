    using UnityEngine;
using UnityEngine.InputSystem;
using System;

public class PlayerInputHandler2D : MonoBehaviour
{
    [Header("Character Input Values")]
    public Vector2 move;
    public Vector2 look;
    public bool jump;
    public bool sprint;

    // Event untuk memberitahu script lain bahwa tombol Interact ditekan
    public event Action OnInteractEvent;

    private PlayerInputAction _inputActions;

    private void Awake()
    {
        _inputActions = new PlayerInputAction();
    }

    private void OnEnable()
    {
        _inputActions.Player.Enable();

        // Movement
        _inputActions.Player.Move.performed += OnMove;
        _inputActions.Player.Move.canceled += OnMove;

        _inputActions.Player.Look.performed += OnLook;
        _inputActions.Player.Look.canceled += OnLook;

        // Jump
        _inputActions.Player.Jump.started += OnJump;
        _inputActions.Player.Jump.canceled += OnJump;

        // Sprint
        _inputActions.Player.Sprint.performed += OnSprint;
        _inputActions.Player.Sprint.canceled += OnSprint;

        // Interact (BARU: Kita tambahkan listener di sini)
        _inputActions.Player.Interact.performed += OnInteract;
    }

    private void OnDisable()
    {
        _inputActions.Player.Move.performed -= OnMove;
        _inputActions.Player.Move.canceled -= OnMove;

        _inputActions.Player.Jump.started -= OnJump;
        _inputActions.Player.Jump.canceled -= OnJump;

        _inputActions.Player.Sprint.performed -= OnSprint;
        _inputActions.Player.Sprint.canceled -= OnSprint;

        // Interact Cleanup
        _inputActions.Player.Interact.performed -= OnInteract;

        _inputActions.Player.Disable();
    }

    #region Input Action Callbacks

    private void OnMove(InputAction.CallbackContext ctx)
    {
        move = ctx.ReadValue<Vector2>();
    }
    private void OnLook(InputAction.CallbackContext ctx)
    {
        look = ctx.ReadValue<Vector2>();
    }

    private void OnJump(InputAction.CallbackContext ctx)    
    {   
        jump = ctx.ReadValueAsButton(); 
    }   

    private void OnSprint(InputAction.CallbackContext ctx)  
    {   
        sprint = ctx.ReadValueAsButton();   
    }

    // Callback khusus Interact
    private void OnInteract(InputAction.CallbackContext ctx)
    {
        // Invoke (panggil) event jika ada script lain yang mendengarkan (seperti InteractHandler2D)
        OnInteractEvent?.Invoke();
        Debug.Log("Interact button pressed.");
    }

    #endregion
}