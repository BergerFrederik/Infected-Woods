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

    public event Action<int> OnWeaponAbilitySlotStarted;

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
        playerInput.Player.UseWeaponAbility1.started += WeaponAbilitySlot1Started;
        playerInput.Player.UseWeaponAbility2.started += WeaponAbilitySlot2Started;
        playerInput.Player.UseWeaponAbility3.started += WeaponAbilitySlot3Started;
        playerInput.Player.UseWeaponAbility4.started += WeaponAbilitySlot4Started;
        playerInput.Player.UseWeaponAbility5.started += WeaponAbilitySlot5Started;
        playerInput.Player.UseWeaponAbility6.started += WeaponAbilitySlot6Started;
    }

    private void OnDisable()
    {
        if (playerInput != null)
        {
            playerInput.MenuControls.Pause.performed -= PausePerformed;
            playerInput.Player.UseDashAbility.started -= DashStarted;
            playerInput.Player.UseAbility.started -= AbilityStarted;
            playerInput.Player.UseAbility.canceled -= AbilityCanceled;
            playerInput.Player.UseWeaponAbility1.started -= WeaponAbilitySlot1Started;
            playerInput.Player.UseWeaponAbility2.started -= WeaponAbilitySlot2Started;
            playerInput.Player.UseWeaponAbility3.started -= WeaponAbilitySlot3Started;
            playerInput.Player.UseWeaponAbility4.started -= WeaponAbilitySlot4Started;
            playerInput.Player.UseWeaponAbility5.started -= WeaponAbilitySlot5Started;
            playerInput.Player.UseWeaponAbility6.started -= WeaponAbilitySlot6Started;

            playerInput.Player.Disable();
            playerInput.MenuControls.Disable();
            playerInput.Dispose();
        }
    }

    private void PausePerformed(InputAction.CallbackContext obj) => OnPausePerformed?.Invoke();
    private void DashStarted(InputAction.CallbackContext obj) => OnDashStarted?.Invoke();
    private void AbilityStarted(InputAction.CallbackContext obj) => OnAbilityStarted?.Invoke();
    private void AbilityCanceled(InputAction.CallbackContext obj) => OnAbilityCanceled?.Invoke();
    private void WeaponAbilitySlot1Started(InputAction.CallbackContext obj) => OnWeaponAbilitySlotStarted?.Invoke(0);
    private void WeaponAbilitySlot2Started(InputAction.CallbackContext obj) => OnWeaponAbilitySlotStarted?.Invoke(1);
    private void WeaponAbilitySlot3Started(InputAction.CallbackContext obj) => OnWeaponAbilitySlotStarted?.Invoke(2);
    private void WeaponAbilitySlot4Started(InputAction.CallbackContext obj) => OnWeaponAbilitySlotStarted?.Invoke(3);
    private void WeaponAbilitySlot5Started(InputAction.CallbackContext obj) => OnWeaponAbilitySlotStarted?.Invoke(4);
    private void WeaponAbilitySlot6Started(InputAction.CallbackContext obj) => OnWeaponAbilitySlotStarted?.Invoke(5);

    public Vector2 GetMovementVectorNormalized()
    {
        if (playerInput == null) return Vector2.zero;
        Vector2 inputVector = playerInput.Player.Move.ReadValue<Vector2>();
        return inputVector.sqrMagnitude > 1 ? inputVector.normalized : inputVector;
    }
}