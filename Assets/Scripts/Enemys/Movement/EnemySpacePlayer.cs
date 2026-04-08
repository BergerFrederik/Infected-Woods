using UnityEngine;

public class EnemySpacePlayer : MonoBehaviour
{
    [SerializeField] private Pathfinder pathfinder;
    [SerializeField] private EnemyStats enemyStats;
    [SerializeField] private EnemyKnockback knockback;
    [SerializeField] private GameObject projectilePrefab;
    [SerializeField] private float spacingDistanceToPlayer, followDistanceToPlayer, attackCooldown, projectileSpeed, prepareTime;

    private float cooldownStarttime, prepareStarttime;
    private enum AttackState { Space, Prepare, Attacking }
    private AttackState currentState = AttackState.Space;

    private void Update()
    {
        float dist = pathfinder.GetDistanceToPlayer();
        Vector2 pathDir = pathfinder.CalculateEnemyMovementVector();
        Vector3 finalMove = Vector3.zero;

        if (currentState == AttackState.Space)
        {
            if (dist < spacingDistanceToPlayer) {
                finalMove = -(Vector3)pathDir * enemyStats.enemyMoveSpeed;
                knockback.useLerpResistance = false; // Flucht = wegfliegen lassen
            }
            else if (dist > followDistanceToPlayer) {
                finalMove = (Vector3)pathDir * enemyStats.enemyMoveSpeed;
                knockback.useLerpResistance = true; // Jagd = Widerstand
            }

            if (Time.time - cooldownStarttime >= attackCooldown)
            {
                prepareStarttime = Time.time;
                currentState = AttackState.Prepare;
            }
        }
        
        if (currentState == AttackState.Prepare && Time.time - prepareStarttime >= prepareTime)
            currentState = AttackState.Attacking;

        if (currentState == AttackState.Attacking)
        {
            AttackPlayer();
            currentState = AttackState.Space;
        }

        knockback.ApplyMovement(finalMove);
    }

    private void AttackPlayer()
    {
        Vector2 shootDir = (pathfinder.GetPlayerPosition() - (Vector2)transform.position).normalized;
        float angle = Mathf.Atan2(shootDir.y, shootDir.x) * Mathf.Rad2Deg;
        GameObject proj = Instantiate(projectilePrefab, transform.position, Quaternion.Euler(0, 0, angle - 90f));
        EnemyProjectile enemyProjectile = proj.GetComponent<EnemyProjectile>();
        
        enemyProjectile.SetEnemyStats(enemyStats);    
        enemyProjectile.Initialize(shootDir, projectileSpeed);
        cooldownStarttime = Time.time;
    }
}