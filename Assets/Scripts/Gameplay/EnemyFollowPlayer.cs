using System.Collections;
using UnityEngine;



public class EnemyFollowPlayer : MonoBehaviour
{
    [SerializeField] private float searchRadius = 0f;
    [SerializeField] private float transition_speed = 0.01f;
    [SerializeField] private float minCurrentWeight = 0.6f;
    [SerializeField] private float maxCurrentWeight = 0f;
    private GameObject player;

    private PlayerStats playerStats;
    private EnemyStats enemyStats;

    private float currentWeight = 1.0f;
    private float knockback_duration = .2f;

    private bool isKnockedBack = false;
    Vector2 moveDirRegardingEnemys;

    float playerKnockback;
    float weaponKnockback;
    float knockbackResistance;
    float knockbackStrength;
    float startTime;


    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        playerStats = player.GetComponent<PlayerStats>();
        enemyStats = transform.GetComponent<EnemyStats>();
    }


    private void Update()
    {
        Debug.Log(isKnockedBack);
        if (!isKnockedBack)
        {
            // Vektor von Gegner zu Spieler
            Vector2 playerPosition = player.transform.position;
            Vector2 enemyPosition = transform.position;
            Vector2 moveDirTowardsPlayer = playerPosition - enemyPosition;

            // Closest Gegner Finden

            Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, searchRadius);
            float closestDistance = Mathf.Infinity;
            float newDistance;
            Transform closestEnemy = null;
            foreach (Collider2D collider in colliders)
            {
                if (collider.CompareTag("Enemy") && collider.gameObject != this.gameObject)
                {
                    newDistance = Vector2.Distance(collider.transform.position, transform.position);
                    if (closestDistance > newDistance && newDistance < searchRadius)
                    {
                        closestEnemy = collider.transform;
                        closestDistance = newDistance;
                    }
                }
            }
            if (closestEnemy != null)
            {
                Vector2 dirToClosestEnemy = closestEnemy.transform.position - transform.position;

                // Vektor von Gegner zu Gegner invertieren
                Vector2 invertedDirToClosestEnemy = dirToClosestEnemy * - 1;

                // Beide Vektoren über lerp verbinden

                float liveWeight = Mathf.Abs(closestDistance / searchRadius);

                if (currentWeight < liveWeight)
                {
                    currentWeight = Mathf.Max(currentWeight - transition_speed * Time.deltaTime, liveWeight);
                }
                else if (currentWeight > liveWeight)
                {
                    currentWeight = Mathf.Min(currentWeight + transition_speed * Time.deltaTime, liveWeight);
                }
                currentWeight = Mathf.Min(currentWeight, minCurrentWeight);
                currentWeight = Mathf.Max(currentWeight, maxCurrentWeight);

                moveDirRegardingEnemys = Vector2.Lerp(invertedDirToClosestEnemy.normalized * (1.0f - liveWeight) * enemyStats.enemyMoveSpeed, moveDirTowardsPlayer.normalized, currentWeight);
            }
            else
            {
                moveDirRegardingEnemys = moveDirTowardsPlayer.normalized;
            }
            
            // Gewichtung abhängig von Gegner Distanz machen
            transform.position += (Vector3)moveDirRegardingEnemys * enemyStats.enemyMoveSpeed * Time.deltaTime;
        }
        else
        {
            KnockEnemyBack();
        }
    }

    private void KnockEnemyBack()
    {       
        Vector3 invertedDirToPlayer = (transform.position - player.transform.position).normalized;          
        transform.position += invertedDirToPlayer * knockbackStrength * Time.deltaTime;
        if (Time.time - startTime > knockback_duration)
        {
            isKnockedBack = false;
        }
    }

    private void OnTriggerEnter2D(Collider2D collider)
    {
        if (collider.TryGetComponent<WeaponStats>(out WeaponStats weaponStats))
        {
            if (!isKnockedBack)
            {
                startTime = Time.time;
                playerKnockback = playerStats.playerKnockback/100;
                weaponKnockback = weaponStats.weaponKnockback;
                knockbackResistance = enemyStats.enemyKnockbackResistance;
                knockbackStrength = (weaponKnockback + weaponKnockback * playerKnockback) * (1 - (knockbackResistance/100));
                isKnockedBack = true;
            }
        }
    }
    private IEnumerator ResetKnockback(float knockbackDuration)
    {
        
        yield return new WaitForSeconds(knockbackDuration);
        
    }
}
