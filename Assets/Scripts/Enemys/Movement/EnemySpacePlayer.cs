using UnityEngine;

public class EnemySpacePlayer : MonoBehaviour
{
    [SerializeField] private Pathfinder pathfinder;
    [SerializeField] private EnemyStats enemyStats;
    [SerializeField] private float spacingDistanceToPlayer = 0f;
    [SerializeField] private float followDistanceToPlayer = 0f;

    private void Update()
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
}