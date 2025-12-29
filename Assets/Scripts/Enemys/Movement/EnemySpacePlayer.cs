using UnityEngine;

public class EnemySpacePlayer : MonoBehaviour
{
    [SerializeField] private Pathfinder pathfinder;
    [SerializeField] private EnemyStats enemyStats;
    [SerializeField] private GameObject projectilePrefab;
    [SerializeField] private float spacingDistanceToPlayer = 0f;
    [SerializeField] private float followDistanceToPlayer = 0f;
    [SerializeField] private float attackCooldown = 0f;
    [SerializeField] private float projectileSpeed = 0f;
    [SerializeField] private float prepareTime = 0f;

    private float cooldownStarttime;
    private float prepareStarttime;
    private bool hasProjectile;

    private Vector2 playerPos;

    private enum AttackState
    {
        Space,
        Prepare,
        Attacking,     
    }

    private AttackState currentState = AttackState.Space;

    private void Awake()
    {
        cooldownStarttime = Time.time - attackCooldown;
        if (projectilePrefab != null)
        {
            hasProjectile = true;
        }
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
            case AttackState.Space:
                if (Time.time - cooldownStarttime >= attackCooldown)
                {
                    prepareStarttime = Time.time;
                    currentState = AttackState.Prepare;
                }
                break;
            case AttackState.Prepare:
                if (Time.time - prepareStarttime >= prepareTime)
                {
                    currentState = AttackState.Attacking;
                }
                break;
            case AttackState.Attacking:
                break;
        }
    }
    
    private void HandleState()
    {
        switch (currentState)
        {
            case AttackState.Space:
                Space();
                break;
            case AttackState.Prepare:
                break;
            case AttackState.Attacking:
                if (hasProjectile)
                {
                    AttackPlayer();
                }
                currentState = AttackState.Space;
                break;
        }
    }

    private void Space()
    {
        float distanceToPlayer = pathfinder.GetDistanceToPlayer();
        Vector2 moveDir = pathfinder.CalculateEnemyMovementVector();
        if (distanceToPlayer < spacingDistanceToPlayer)
        {
            transform.position -= (Vector3)moveDir * enemyStats.enemyMoveSpeed * Time.deltaTime;
        }
        else if (distanceToPlayer > followDistanceToPlayer)
        {
            transform.position += (Vector3)moveDir * enemyStats.enemyMoveSpeed * Time.deltaTime;
        }
    }

    private void AttackPlayer()
    {
        playerPos = pathfinder.GetPlayerPosition();
        Vector2 shootDirection = (playerPos - (Vector2)transform.position).normalized;
        float angle = Mathf.Atan2(shootDirection.y, shootDirection.x) * Mathf.Rad2Deg;
        GameObject projectileInstance = Instantiate(projectilePrefab, transform.position, Quaternion.Euler(0, 0, angle - 90f));
        EnemyProjectile enemyProjectile = projectileInstance.GetComponent<EnemyProjectile>();     
        enemyProjectile.SetEnemyStats(enemyStats);
        enemyProjectile.Initialize(shootDirection, projectileSpeed);
        cooldownStarttime = Time.time;
    }
}