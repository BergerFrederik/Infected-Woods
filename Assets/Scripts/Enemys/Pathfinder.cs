using System.Collections;
using UnityEngine;



public class Pathfinder : MonoBehaviour
{
    [SerializeField] private float searchRadius = 0f;
    [SerializeField] private float transition_speed = 0.01f;
    [SerializeField] private float minCurrentWeight = 0.6f;
    [SerializeField] private float maxCurrentWeight = 0f;
    [SerializeField] private Transform enemyTransform;
    [SerializeField] private float enemyRadius = 0f;
    [SerializeField] private Transform enemyVisuals;

    private GameObject player;
    private GameObject gameManager;
    private PlayerStats playerStats;
    private EnemyStats enemyStats;
    private GameManager gameManagerScript;



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
        gameManager = GameObject.FindGameObjectWithTag("Manager");
        playerStats = player.GetComponent<PlayerStats>();
        enemyStats = transform.GetComponent<EnemyStats>();
        gameManagerScript = gameManager.GetComponent<GameManager>();
    }


    private void Update()
    {
        if (isKnockedBack)
        {
            KnockEnemyBack();            
        }
        else
        {
            Vector3 currentScale = enemyVisuals.localScale;
            SetSpriteDirection(CalculateEnemyMovementVector(), currentScale);
        }
    }

    public Vector2 CalculateEnemyMovementVector()
    {
        if (player == null)
        {
            return Vector2.zero;
        }
        
        Vector2 playerPosition = player.transform.position;
        Vector2 enemyPosition = transform.position;
        Vector2 moveDirTowardsPlayer = playerPosition - enemyPosition;

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
            Vector2 invertedDirToClosestEnemy = dirToClosestEnemy * -1;

            // Beide Vektoren �ber lerp verbinden

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
            moveDirRegardingEnemys = moveDirTowardsPlayer;
        }
        return moveDirRegardingEnemys.normalized;
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
                playerKnockback = playerStats.playerKnockback / 100;
                weaponKnockback = weaponStats.weaponKnockback;
                knockbackResistance = enemyStats.enemyKnockbackResistance;
                knockbackStrength = (weaponKnockback + weaponKnockback * playerKnockback) * (1 - (knockbackResistance / 100));
                if (knockbackStrength > 0)
                {
                    isKnockedBack = true;
                }
            }
        }
    }
    private void SetSpriteDirection(Vector2 moveDir, Vector3 currentScale)
    {
        if (moveDir.x > 0 && currentScale.x > 0)
        {
            currentScale.x *= -1;
        }
        else if (moveDir.x < 0 && currentScale.x < 0)
        {
            currentScale.x *= -1;
        }
        enemyVisuals.localScale = currentScale;
    }

    private void LateUpdate()
    {
        if (!this.enabled) return;
        ClampToMapBounds(gameManagerScript.mapSize);
    }
    private void ClampToMapBounds(Bounds mapBounds)
    {
        Vector3 currentPos = this.transform.position;

        float minX = mapBounds.min.x + enemyRadius;
        float minY = (mapBounds.min.y + enemyRadius) * 0.97f;
        float maxX = mapBounds.max.x - enemyRadius;
        float maxY = (mapBounds.max.y - enemyRadius) * 1.03f;

        currentPos.x = Mathf.Clamp(currentPos.x, minX, maxX);
        currentPos.y = Mathf.Clamp(currentPos.y, minY, maxY);
        this.transform.position = currentPos;
    }

    public float GetDistanceToPlayer()
    {
        Vector2 playerPosition = player.transform.position;
        Vector2 enemyPosition = transform.position;
        float distanceToPlayer = Vector2.Distance(enemyPosition, playerPosition);
        return distanceToPlayer;
    }

    public Vector2 GetPlayerPosition()
    {
        Vector2 playerPosition = player.transform.position;
        return playerPosition;
    }

    public GameObject GetPlayerObject()
    {
        return player;
    }
}
