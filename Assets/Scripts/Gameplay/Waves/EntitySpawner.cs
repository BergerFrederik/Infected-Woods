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
    [SerializeField] private WaveManager waveManager;
    
    [Header("Spawn Distances")]
    [SerializeField] private float minGroupDist = 10f;
    [SerializeField] private float maxGroupDist = 15f;

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

    public void AddGroupToQueue(WaveGroupData group)
    {
        if (_isCleaningUp) return;

        // 1. Check: Haben wir das Limit aus WaveData bereits erreicht?
        // Wir zählen aktive Gegner + die, die bereits in der Warteschlange stehen.
        int currentTotal = activeEnemies.Count + queueToSpawn.Count;
        int limit = (waveManager.currentWave != null) ? waveManager.currentWave.maxEnemies : 100;

        if (currentTotal >= limit) return;

        Vector2 groupCenter = CalculateGroupCenter(group);

        foreach (var unit in group.waveUnitsData)
        {
            if (Random.Range(0f, 1f) > unit.spawnChance) continue;

            int count = Random.Range((int)unit.minNumber, (int)unit.maxNumber + 1);
            
            for (int i = 0; i < count; i++)
            {
                // Zweiter Check innerhalb der Schleife, falls die Gruppe das Limit sprengt
                if (activeEnemies.Count + queueToSpawn.Count >= limit) break;

                // Wir übergeben jetzt auch die unit-Daten für den individuellen Distanz-Check
                Vector2 spawnPos = CalculateValidSpawnPos(groupCenter, group, unit);
                
                queueToSpawn.Add(new SpawnRequest { 
                    prefab = unit.enemyUnit, 
                    position = spawnPos 
                });
            }
        }
    }

    private Vector2 CalculateGroupCenter(WaveGroupData group)
    {
        if (group.spawnEdgeOfMap) return playerTransform.position;

        Vector2 randomDir = Random.insideUnitCircle.normalized;
        float randomDistance = Random.Range(minGroupDist, maxGroupDist);
    
        return (Vector2)playerTransform.position + randomDir * randomDistance;
    }

    private Vector2 CalculateValidSpawnPos(Vector2 center, WaveGroupData group, WaveDataUnit unit)
    {
        Vector2 finalPos = center;
        
        // Brotato-Logik: Basis-Distanz + individueller Bonus der Unit
        float currentMinDist = initialMinDistFromPlayer + unit.additionalMinDistanceFromPlayer;
        
        int attempts = 0;
        bool validFound = false;
        Bounds b = gameManager.mapSize;

        while (!validFound && attempts < 15)
        {
            if (group.spawnEdgeOfMap)
            {
                finalPos = GetRandomEdgePosition(b, group.spawnDistAwayFromEdges);
            }
            else if (group.area < 0)
            {
                finalPos = new Vector2(
                    Random.Range(b.min.x + 1f, b.max.x - 1f),
                    Random.Range(b.min.y + 1f, b.max.y - 1f)
                );
            }
            else
            {
                finalPos = center + Random.insideUnitCircle * group.area;
            }

            finalPos.x = Mathf.Clamp(finalPos.x, b.min.x + 1f, b.max.x - 1f);
            finalPos.y = Mathf.Clamp(finalPos.y, b.min.y + 1f, b.max.y - 1f);

            if (Vector2.Distance(finalPos, playerTransform.position) > currentMinDist)
            {
                validFound = true;
            }
            else
            {
                // Wenn kein Platz frei ist, Distanzanspruch leicht senken (Brotato-Algorithmus)
                currentMinDist = Mathf.Max(3f, currentMinDist - 0.5f);
                attempts++;
            }
        }
        return finalPos;
    }

    private Vector2 GetRandomEdgePosition(Bounds b, float edgeOffset)
    {
        int side = Random.Range(0, 4);
        // Wir nutzen hier dein Feld 'spawnDistAwayFromEdges'
        float offset = Mathf.Max(1f, edgeOffset); 

        switch (side)
        {
            case 0: return new Vector2(Random.Range(b.min.x, b.max.x), b.max.y - offset);
            case 1: return new Vector2(b.max.x - offset, Random.Range(b.min.y, b.max.y));
            case 2: return new Vector2(Random.Range(b.min.x, b.max.x), b.min.y + offset);
            case 3: return new Vector2(b.min.x + offset, Random.Range(b.min.y, b.max.y));
            default: return b.center;
        }
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

    public void RegisterEnemy(GameObject enemy)
    {
        if (enemy != null) activeEnemies.Add(enemy);
    }

    // WICHTIG: Wenn ein Gegner stirbt, muss er sich hier abmelden!
    public void UnregisterEnemy(GameObject enemy)
    {
        if (activeEnemies.Contains(enemy)) activeEnemies.Remove(enemy);
    }
    
    public void ClearEnemies()
    {
        _isCleaningUp = true;
        queueToSpawn.Clear();
        for (int i = activeEnemies.Count - 1; i >= 0; i--)
        {
            if (activeEnemies[i] != null) Destroy(activeEnemies[i]);
        }
        activeEnemies.Clear();
        _isCleaningUp = false;
    }
}