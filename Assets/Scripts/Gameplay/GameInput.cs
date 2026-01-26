using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class GameInput : MonoBehaviour
{
    public PlayerInput playerInput;
    
    public event Action OnPausePerformed;
    public event Action OnDashStarted;
    
    public event Action OnAbilityStarted;
    public event Action OnAbilityCanceled;

    private void Awake()
    {               
        playerInput = new PlayerInput();
        playerInput.Player.Enable(); 
        playerInput.MenuControls.Enable();
    }

    private void OnEnable()
    {
        playerInput.MenuControls.Pause.performed += PausePerformed;
        playerInput.Player.UseDashAbility.started += DashStarted;
        playerInput.Player.UseAbility.started += AbilityStarted;
        playerInput.Player.UseAbility.canceled += AbilityCanceled;
    }

    private void OnDisable()
    {
        if (playerInput != null)
        {
            playerInput.MenuControls.Pause.performed -= PausePerformed;
            playerInput.Player.UseDashAbility.started -= DashStarted;
            playerInput.Player.UseAbility.started -= AbilityStarted;
            playerInput.Player.UseAbility.canceled -= AbilityCanceled;

            playerInput.Player.Disable();
            playerInput.MenuControls.Disable();
            playerInput.Dispose();
        }
    }

    private void PausePerformed(InputAction.CallbackContext obj) => OnPausePerformed?.Invoke();
    private void DashStarted(InputAction.CallbackContext obj) => OnDashStarted?.Invoke();
    private void AbilityStarted(InputAction.CallbackContext obj) => OnAbilityStarted?.Invoke();
    private void AbilityCanceled(InputAction.CallbackContext obj) => OnAbilityCanceled?.Invoke();

    public Vector2 GetMovementVectorNormalized()
    {
        if (playerInput == null) return Vector2.zero;
        Vector2 inputVector = playerInput.Player.Move.ReadValue<Vector2>();
        return inputVector.sqrMagnitude > 1 ? inputVector.normalized : inputVector;
    }
}