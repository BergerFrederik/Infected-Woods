using UnityEngine;



public class EnemyFollowPlayer : MonoBehaviour
{
    [SerializeField] private EnemyStats enemyStats;
    [SerializeField] private Pathfinder pathfinder;

    private void Update()
    {
        Vector2 moveDir = pathfinder.CalculateEnemyMovementVector();
        transform.position += (Vector3)moveDir * enemyStats.enemyMoveSpeed * Time.deltaTime;
    }
}


    