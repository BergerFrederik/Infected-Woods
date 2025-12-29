using System.Collections;
using UnityEngine;

public class NormalWave : MonoBehaviour
{
    [SerializeField] private WaveStats waveStats;
    [SerializeField] private GameObject SpawnIndicatorPrefab;
    [SerializeField] private WaveEnemyHandler waveEnemyHandler;
    [SerializeField] private float spawn_indicator_delay;
    [SerializeField] GameObject[] enemyPrefabs;
    [SerializeField] private float spawn_start_interval;
    [SerializeField] private float spawn_end_interval;
    [SerializeField] private int spawnlimit_at_once;
    [SerializeField] private float enemy_limit;
    
    private float elapsedTime;
    private float startTime;
    
    private void Start()
    {
        StartCoroutine(SpawnLoop(computeInterval()));
        startTime = Time.time;
    }
    
    private void OnDestroy()
    {
        StopAllCoroutines();
    }

    private float computeInterval()
    {
        float progress = elapsedTime / waveStats.waveDuration;
        return Mathf.Lerp(spawn_start_interval, spawn_end_interval, progress);
    }
    
    private IEnumerator SpawnLoop(float interval)
    {
        while (true)
        {
            elapsedTime = Time.time - startTime;
            int randomIndex = Random.Range(0, enemyPrefabs.Length);
            GameObject enemy = enemyPrefabs[randomIndex];
            int randomNumSpawns = Random.Range(2, spawnlimit_at_once + 1);
            for (int i = 1; i <= randomNumSpawns; i++)
            {
                Vector2 spawnPosition = waveEnemyHandler.GetSpawnPosition();
                StartCoroutine(PerformSpawn(enemy, spawnPosition));
            }
            yield return new WaitForSeconds(interval);
        }
    }

    private IEnumerator PerformSpawn(GameObject enemy, Vector2 spawnPosition)
    {
        GameObject SpawnIndicator = Instantiate(SpawnIndicatorPrefab, new Vector3(spawnPosition.x, spawnPosition.y, 0), Quaternion.identity);
        yield return new WaitForSeconds(spawn_indicator_delay);
        Destroy(SpawnIndicator);
        GameObject NewEnemy = Instantiate(enemy, new Vector3(spawnPosition.x, spawnPosition.y, 0), Quaternion.identity);
    }
}
