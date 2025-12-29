using UnityEngine;

public class WaveEnemyHandler : MonoBehaviour
{
    [SerializeField] private float maxSpawnDistance;
    [SerializeField] private float minSpawnDistance;
    
    private GameObject PlayerObject;
    private GameManager gameManager;
    private Bounds mapBounds;
    private void Awake()
    {
        PlayerObject = GameObject.FindGameObjectWithTag("Player");
        gameManager = GameObject.FindGameObjectWithTag("Manager").GetComponent<GameManager>();
        mapBounds = gameManager.mapSize;
    }
    
    public Vector2 GetSpawnPosition()
    {
        Vector2 randomDir = UnityEngine.Random.insideUnitCircle.normalized;
        float randomDistance = Random.Range(minSpawnDistance, maxSpawnDistance);
        Vector2 spawnPosition = (Vector2)PlayerObject.transform.position + (randomDir * randomDistance);
        spawnPosition.x = Mathf.Clamp(spawnPosition.x, mapBounds.min.x * 0.95f, mapBounds.max.x * 0.95f);
        spawnPosition.y = Mathf.Clamp(spawnPosition.y, mapBounds.min.y * 0.95f, mapBounds.max.y * 0.95f);
        return spawnPosition;
    }
}
