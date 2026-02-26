using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class EntitySpawner : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private GameObject entityBirthPrefab;
    [SerializeField] private float spawnDelay = 0.05f;
    [SerializeField] private float initialMinDistFromPlayer = 8f;

    [Header("References")]
    [SerializeField] private GameManager gameManager;
    [SerializeField] private Transform playerTransform;
    
    [Header("Spawn Distances")]
    [SerializeField] private float minGroupDist = 8f;
    [SerializeField] private float maxGroupDist = 13f;

    private List<GameObject> activeEnemies = new List<GameObject>();
    private List<SpawnRequest> queueToSpawn = new List<SpawnRequest>();
    private bool _isCleaningUp = false;

    private struct SpawnRequest 
    {
        public GameObject prefab;
        public Vector2 position;
    }

    private void Start()
    {
        StartCoroutine(ProcessQueueLoop());
    }

    // Entspricht on_group_spawn_timing_reached
    public void AddGroupToQueue(WaveGroupData group)
    {
        if (_isCleaningUp) return;

        // 1. Gruppen-Zentrum bestimmen (wie get_group_pos)
        Vector2 groupCenter = CalculateGroupCenter(group);

        // 2. Einheiten der Gruppe verarbeiten
        foreach (var unit in group.waveUnitsData)
        {
            int count = Random.Range(unit.minNumber, unit.maxNumber + 1);
            for (int i = 0; i < count; i++)
            {
                // Nutzt jetzt deinen neuen spawnRadius!
                Vector2 spawnPos = CalculateValidSpawnPos(groupCenter, group);
                
                queueToSpawn.Add(new SpawnRequest { 
                    prefab = unit.enemyPrefab, 
                    position = spawnPos 
                });
            }
        }
    }

    private Vector2 CalculateGroupCenter(WaveGroupData group)
    {
        Vector2 randomDir = Random.insideUnitCircle.normalized;
        float randomDistance = Random.Range(minGroupDist, maxGroupDist);
    
        Vector2 pos = (Vector2)playerTransform.position + randomDir * randomDistance;
        return pos;;
    }

    private Vector2 CalculateValidSpawnPos(Vector2 center, WaveGroupData group)
    {
        Vector2 finalPos = center;
        float currentMinDist = initialMinDistFromPlayer;
        
        int attempts = 0;
        bool validFound = false;

        while (!validFound && attempts < 10)
        {
            // 1. Punkt im Gruppen-Radius würfeln (Entspricht get_spawn_pos_in_area)
            finalPos = center + Random.insideUnitCircle * group.spawnRadius;

            // 2. Map-Bounds Check (Damit er nicht außerhalb der Welt landet)
            Bounds b = gameManager.mapSize;
            finalPos.x = Mathf.Clamp(finalPos.x, b.min.x + 1f, b.max.x - 1f);
            finalPos.y = Mathf.Clamp(finalPos.y, b.min.y + 1f, b.max.y - 1f);

            // 3. Distanz-Check zum Spieler
            float distSq = (finalPos - (Vector2)playerTransform.position).sqrMagnitude;
        
            if (distSq > currentMinDist * currentMinDist)
            {
                validFound = true; // Punkt ist weit genug weg!
            }
            else
            {
                // Wenn zu nah: Verringere den Anspruch etwas (wie im Godot Code: max(25, min_dist - 5))
                currentMinDist = Mathf.Max(2f, currentMinDist - 0.5f);
                attempts++;
            }
        }

        return finalPos;
    }

    private IEnumerator ProcessQueueLoop()
    {
        while (true)
        {
            if (queueToSpawn.Count > 0 && !_isCleaningUp)
            {
                SpawnRequest req = queueToSpawn[0];
                queueToSpawn.RemoveAt(0);
                
                GameObject birth = Instantiate(entityBirthPrefab, req.position, Quaternion.identity);
                birth.GetComponent<EntityBirth>().Setup(req.prefab, this);
            }
            yield return new WaitForSeconds(spawnDelay);
        }
    }

    public void RegisterEnemy(GameObject enemy) => activeEnemies.Add(enemy);
    
    public void ClearEnemies()
    {
        _isCleaningUp = true;
        queueToSpawn.Clear();
        foreach (var e in activeEnemies) if (e != null) Destroy(e);
        activeEnemies.Clear();
        _isCleaningUp = false;
    }
}