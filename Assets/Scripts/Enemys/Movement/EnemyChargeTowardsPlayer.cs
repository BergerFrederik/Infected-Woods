using UnityEngine;

public class EnemyChargeTowardsPlayer : MonoBehaviour
{
    [SerializeField] private float chargeDistanceToPlayer = 1.5f;
    [SerializeField] private float chargeSpeedMultiplier = 2f;
    [SerializeField] private float chargeCooldown = 5f;
    [SerializeField] private float charge_overshoot = 0.35f;
    [SerializeField] private float prepare_time = 1f;
    [SerializeField] private float rest_time = 1;
    [SerializeField] Pathfinder pathfinder;
    [SerializeField] EnemyStats enemyStats;
    [SerializeField] EnemyKnockback knockback;

    private float cooldownStarttime, prepareStartTime, restStartTime;
    private Vector2 moveDir, startPosition;
    public enum ChargeState { Walking, Preparing, Charging, Resting }
    public ChargeState currentState = ChargeState.Walking;

    private void Start() => cooldownStarttime = -chargeCooldown;

    private void Update()
    {
        UpdateState();
        HandleState();
    }

    private void UpdateState()
    {
        switch (currentState)
        {
            case ChargeState.Walking:
                if (Time.time - cooldownStarttime >= chargeCooldown && pathfinder.GetDistanceToPlayer() <= chargeDistanceToPlayer)
                {
                    currentState = ChargeState.Preparing;
                    prepareStartTime = Time.time;
                }
                break;
            case ChargeState.Preparing:
                if (Time.time - prepareStartTime >= prepare_time)
                {
                    currentState = ChargeState.Charging;
                    startPosition = transform.position;
                    moveDir = pathfinder.CalculateEnemyMovementVector();
                }
                break;
            case ChargeState.Charging:
                float dist = chargeDistanceToPlayer * (1 + charge_overshoot);
                if (Vector2.Distance(transform.position, startPosition) >= dist)
                {
                    restStartTime = Time.time;
                    currentState = ChargeState.Resting;
                }
                break;
            case ChargeState.Resting:
                if (Time.time - restStartTime >= rest_time)
                {
                    currentState = ChargeState.Walking;
                    cooldownStarttime = Time.time;
                }
                break;
        }
    }

    private void HandleState()
    {
        switch (currentState)
        {
            case ChargeState.Walking:
                knockback.canReceiveKnockback = true;
                knockback.useLerpResistance = true;
                Vector2 walkDir = pathfinder.CalculateEnemyMovementVector();
                knockback.ApplyMovement((Vector3)walkDir * enemyStats.enemyMoveSpeed);
                break;
            case ChargeState.Preparing:
            case ChargeState.Resting:
                knockback.canReceiveKnockback = false; // Immun während Animationen
                break;
            case ChargeState.Charging:
                knockback.canReceiveKnockback = false; // Während Charge nicht unterbrechbar
                transform.position += (Vector3)moveDir * enemyStats.enemyMoveSpeed * chargeSpeedMultiplier * Time.deltaTime;
                break;
        }
    }
}