using UnityEngine;

public class EnemyFollowPlayer : MonoBehaviour
{
    [SerializeField] private EnemyStats enemyStats;
    [SerializeField] private Pathfinder pathfinder;
    [SerializeField] private EnemyKnockback knockback;

    private void Update()
    {
        Vector2 moveDir = pathfinder.CalculateEnemyMovementVector();
        Vector3 moveVector = (Vector3)moveDir * enemyStats.enemyMoveSpeed;

        knockback.useLerpResistance = true;
        knockback.ApplyMovement(moveVector);
    }
}