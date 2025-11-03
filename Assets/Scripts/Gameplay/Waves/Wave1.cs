using System.Collections;
using UnityEngine;

public class Wave1 : MonoBehaviour
{
    [SerializeField] private WaveStats waveStats;
    [SerializeField] private GameObject enemySpawner;
    public GameObject[] enemyPrefabs;


    float waveDuration;
    float amountEnemyDommshroom;
    float amountEnemyInfectedSnail;

    void Start()
    {
        waveDuration = waveStats.waveDuration;
        amountEnemyDommshroom = waveStats.amountEnemyDoomshroom;
        amountEnemyInfectedSnail = waveStats.amountEnemyInfectedSnail;
        


        StartCoroutine(spawnEnemy(0.5f, enemyPrefabs[0]));
    }

    // Update is called once per frame
    private void Update()
    {
        waveDuration -= Time.deltaTime;
        if (waveDuration < 0f)
        {
            waveStats.waveDuration = 0;
            Destroy(this.gameObject);
        }
    }

    private IEnumerator spawnEnemy(float interval, GameObject enemy)
    {
        yield return new WaitForSeconds(interval);
        GameObject newEnemy = Instantiate(enemy, new Vector3(Random.Range(-5f, 5f), Random.Range(-6f, 6f), 0), Quaternion.identity);
        StartCoroutine(spawnEnemy(interval, enemy));
    }
}
