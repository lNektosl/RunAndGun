using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInput : MonoBehaviour {

    public event EventHandler OnShiftPressed;
    public event EventHandler OnShootStart;
    public event EventHandler OnShootEnd;
    private PlayerIntputActions inputActions;


    public void Awake() {
        inputActions = new PlayerIntputActions();

        inputActions.Player.Enable();
        inputActions.Player.Dash.performed += HandleDashPressed;
        inputActions.Player.Shoot.started += HandleShootStart;
        inputActions.Player.Shoot.canceled += HandleShootCanceled;

    }

    private void HandleShootCanceled(InputAction.CallbackContext obj) {
        OnShootEnd?.Invoke(this, EventArgs.Empty);
    }

    private void HandleShootStart(InputAction.CallbackContext obj) {
        OnShootStart?.Invoke(this, EventArgs.Empty);
    }

    private void HandleDashPressed(InputAction.CallbackContext obj) {
        OnShiftPressed?.Invoke(this, EventArgs.Empty);
    }

    public Vector2 GetVector2Normalized() {
        Vector2 inputVector = inputActions.Player.Move.ReadValue<Vector2>();
        inputVector = inputVector.normalized;
        return inputVector;
    }
}
