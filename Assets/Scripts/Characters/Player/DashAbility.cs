using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class DashAbility : MonoBehaviour
{
    [SerializeField] private PlayerStats playerStats;
    [SerializeField] private Player player;
    [SerializeField] private PlayerMovement playerMovement;
    [SerializeField] private float dash_multiplier = 0f;
    [SerializeField] private float dash_duration = 0f;
    [SerializeField] private float dash_base_speed = 0f;
    [SerializeField] private float dash_base_cooldown = 2f;

    private PlayerInput gameInput;
    private Animator animator;

    private enum DashingState
    {
        dashReady,
        dashing,
        cooldown
    }

    private DashingState currentState = DashingState.dashReady;

    private void Awake()
    {
        //animator = GetComponentInChildren<Animator>();
        gameInput = new PlayerInput();
    }

    private void OnEnable()
    {
        gameInput.Enable();
        gameInput.Player.UseDashAbility.performed += OnUseDashPerformed;
    }

    private void OnDisable()
    {
        gameInput.Player.UseDashAbility.performed -= OnUseDashPerformed;
        gameInput.Disable();
    }

    private void OnUseDashPerformed(InputAction.CallbackContext context)
    {
        if (currentState == DashingState.dashReady)
        {
            CharacterAbilityExecution();
        }
    }

    private void CharacterAbilityExecution()
    {
        //animator.SetTrigger("AbilityPressed");
        currentState = DashingState.dashing;       
        StartCoroutine(RollCoroutine());
    }


    private IEnumerator RollCoroutine()
    {
        float startTime = Time.time;
        float playerMoveSpeed = playerStats.playerBaseMovespeed + playerStats.playerBaseMovespeed * (playerStats.playerMovespeed / 100);
        float rollSpeed = playerMoveSpeed * dash_multiplier + dash_base_speed;

        Vector3 rollDir = playerMovement.CurrentMovementInput;
        if (rollDir != Vector3.zero)
        {
            while (Time.time < startTime + dash_duration)
            {
                player.transform.position += rollDir * rollSpeed * Time.deltaTime;
                yield return null;
            }
            StartCoroutine(ApplyCooldown());
        }
        currentState = DashingState.dashReady;
    }

    
    private IEnumerator ApplyCooldown()
    {
        currentState = DashingState.cooldown;
        float cooldownReduction = playerStats.playerDashCooldownReduction / 100;
        float dashCooldown = dash_base_cooldown - dash_base_cooldown * cooldownReduction;
        yield return new WaitForSeconds(dashCooldown);
        currentState = DashingState.dashReady;
    }
}

