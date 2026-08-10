using System.Collections;
using UnityEngine;



public class Pathfinder : MonoBehaviour
{
    [SerializeField] private EnemyMovementConfig movementConfig;
    [SerializeField] private Transform enemyTransform;
    [SerializeField] private float enemyRadius = 0f;
    [SerializeField] private Transform visualFlipAnchor;

    private GameObject player;
    private GameObject gameManager;
    private PlayerStats playerStats;
    private GameManager gameManagerScript;

    private Vector2 cachedMoveDirection;
    private int cachedFrame = -1;
    private float lastFlipTime = -Mathf.Infinity;

    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        gameManager = GameObject.FindGameObjectWithTag("Manager");
        playerStats = player.GetComponent<PlayerStats>();
        gameManagerScript = gameManager.GetComponent<GameManager>();
    }


    private void Update()
    {
        Vector3 currentScale = visualFlipAnchor.localScale;
        SetSpriteDirection(CalculateEnemyMovementVector(), currentScale);
    }

    public Vector2 CalculateEnemyMovementVector()
    {
        // Mehrere Skripte fragen pro Frame nach der Richtung (Sprite-Flip, Movement-States) -
        // pro Frame nur einmal tatsächlich berechnen.
        if (cachedFrame == Time.frameCount)
        {
            return cachedMoveDirection;
        }
        cachedFrame = Time.frameCount;
        cachedMoveDirection = ComputeMoveDirection();
        return cachedMoveDirection;
    }

    private Vector2 ComputeMoveDirection()
    {
        if (player == null)
        {
            return Vector2.zero;
        }

        Vector2 enemyPosition = transform.position;
        Vector2 playerPosition = player.transform.position;
        Vector2 toPlayer = playerPosition - enemyPosition;
        float distanceToPlayer = toPlayer.magnitude;
        // Bei sehr kleinem Abstand ist die normalisierte Richtung numerisch instabil
        // (kleinstes Zittern in der Position kippt die Richtung um) - Seek-Anteil dann kappen.
        Vector2 seekDir = distanceToPlayer > movementConfig.minSeekDistance ? toPlayer / distanceToPlayer : Vector2.zero;

        Collider2D[] colliders = Physics2D.OverlapCircleAll(enemyPosition, movementConfig.searchRadius, movementConfig.enemyLayerMask);
        Vector2 separation = Vector2.zero;
        int neighborCount = 0;

        foreach (Collider2D collider in colliders)
        {
            if (collider.gameObject == this.gameObject || !collider.CompareTag("Enemy"))
            {
                continue;
            }

            Vector2 offset = enemyPosition - (Vector2)collider.transform.position;
            float distance = offset.magnitude;
            if (distance <= 0f)
            {
                continue;
            }

            // Näher dran = stärkerer Schub weg von diesem Nachbarn
            float weight = 1f - Mathf.Clamp01(distance / movementConfig.searchRadius);
            separation += (offset / distance) * weight;
            neighborCount++;
        }

        if (neighborCount == 0)
        {
            return seekDir;
        }

        separation /= neighborCount;
        float avoidanceWeight = Mathf.Clamp(separation.magnitude, movementConfig.minAvoidanceWeight, movementConfig.maxAvoidanceWeight);

        return Vector2.Lerp(seekDir, separation.normalized, avoidanceWeight).normalized;
    }

    
    private void SetSpriteDirection(Vector2 moveDir, Vector3 currentScale)
    {
        // Kurzes Cooldown gegen Flip-Flackern, wenn die Bewegungsrichtung durch
        // nahe Nachbarn/Spieler kurzzeitig instabil wird.
        if (Time.time - lastFlipTime < movementConfig.minFlipInterval)
        {
            return;
        }

        bool shouldFlip = (moveDir.x > 0 && currentScale.x > 0) || (moveDir.x < 0 && currentScale.x < 0);
        if (shouldFlip)
        {
            currentScale.x *= -1;
            visualFlipAnchor.localScale = currentScale;
            lastFlipTime = Time.time;
        }
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
