using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class GameInput : MonoBehaviour
{
    private PlayerInput playerInput;
    public event Action OnPausePerformed;

    private void Awake()
    {               
        playerInput = new PlayerInput();
        playerInput.Player.Enable(); 
        playerInput.MenuControls.Enable();
    }

    private void OnEnable()
    {
        playerInput.MenuControls.Pause.performed += PausePerformed;
    }

    private void OnDisable()
    {
        if (playerInput != null)
        {
            playerInput.Player.Disable();
            playerInput.MenuControls.Disable();
            playerInput.Dispose();
            playerInput.MenuControls.Pause.performed -= PausePerformed;
        }
    }

    private void PausePerformed(InputAction.CallbackContext obj)
    {
        OnPausePerformed?.Invoke();
        Debug.Log("pressed");
    }

    public Vector2 GetMovementVectorNormalized()
    {
        if (playerInput == null) return Vector2.zero;
        Vector2 inputVector = playerInput.Player.Move.ReadValue<Vector2>();
        if (inputVector.sqrMagnitude > 1)
        {
            inputVector = inputVector.normalized;
        }
        return inputVector;
    }

    public bool IsDashPressed()
    {
        if (playerInput == null) return false;
        return playerInput.Player.UseDashAbility.WasPressedThisFrame();
    }

    public bool IsAbilityPressed()
    {
        if (playerInput == null) return false;
        return playerInput.Player.UseAbility.WasPressedThisFrame();
    }
}
