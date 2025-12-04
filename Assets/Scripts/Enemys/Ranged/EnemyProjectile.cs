using UnityEngine;

public class EnemyProjectile : MonoBehaviour
{
    private Vector2 direction;
    private float speed;
    private EnemyStats enemyStats;

    public void SetEnemyStats(EnemyStats projectileEnemyStats)
    {
        enemyStats = projectileEnemyStats;
    }
    public void Initialize(Vector2 shotDirection, float shotSpeed)
    {
        direction = shotDirection;
        speed = shotSpeed;
    }

    public EnemyStats GetEnemyStats()
    {
        return enemyStats;
    }

    private void Update()
    {
        if (speed > 0)
        {
            transform.position += (Vector3)direction * speed * Time.deltaTime;
        }
    }

    private void OnTriggerEnter2D(Collider2D collider)
    {
        if (collider.gameObject.CompareTag("Floor") || collider.gameObject.CompareTag("Player"))
        {
            Destroy(this.gameObject);
        }
    }
}
