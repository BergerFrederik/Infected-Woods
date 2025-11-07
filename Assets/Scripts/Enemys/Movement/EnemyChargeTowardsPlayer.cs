using System;
using System.Collections;
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

    private float cooldownStarttime;
    private float prepareStartTime;
    private float restStartTime;

    private Vector2 moveDir;
    private Vector2 startPosition;

    public ChargeState currentState = ChargeState.Walking;

    public enum ChargeState
    {
        Walking,        
        Preparing,
        Charging,
        Resting
    }

    private void Start()
    {
        cooldownStarttime = -chargeCooldown;
    }
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
                var distanceToPlayer = pathfinder.GetDistanceToPlayer();
                var hasNoCooldown = Time.time - cooldownStarttime >= chargeCooldown;
                var isCloseEnough = distanceToPlayer <= chargeDistanceToPlayer;
                if (hasNoCooldown && isCloseEnough)
                {
                    currentState = ChargeState.Preparing;
                    prepareStartTime = Time.time;
                }
                break;
            case ChargeState.Preparing:
                float passedPrepareTime = Time.time - prepareStartTime;
                if (passedPrepareTime >= prepare_time)
                {
                    currentState = ChargeState.Charging;
                    startPosition = transform.position;
                    moveDir = pathfinder.CalculateEnemyMovementVector();
                }
                break;
            case ChargeState.Charging:
                Vector2 currentPosition = transform.position;               
                float chargeDistance = chargeDistanceToPlayer + chargeDistanceToPlayer * charge_overshoot;
                if (Vector2.Distance(currentPosition, startPosition) >= chargeDistance)
                {
                    restStartTime = Time.time;
                    currentState = ChargeState.Resting;
                }
                break;
            case ChargeState.Resting:
                float passedRestTime = Time.time - restStartTime;
                if (passedRestTime >= rest_time)
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
                WalkTowardsEnemy();
                break;
            case ChargeState.Preparing:
                break;
            case ChargeState.Charging:
                ChargeTowardsPlayer();
                break;
            case ChargeState.Resting:
                break;
        }
    }

    private void WalkTowardsEnemy()
    {
        moveDir = pathfinder.CalculateEnemyMovementVector();
        transform.position += (Vector3)moveDir * enemyStats.enemyMoveSpeed * Time.deltaTime;
    }


    private void ChargeTowardsPlayer()
    {      
        transform.position += (Vector3)moveDir * enemyStats.enemyMoveSpeed * chargeSpeedMultiplier * Time.deltaTime;
    }
}
