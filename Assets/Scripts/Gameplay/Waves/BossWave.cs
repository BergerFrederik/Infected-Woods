using UnityEngine;

public class BossWave : MonoBehaviour
{
    [SerializeField] private GameObject bossPrefab;
    [SerializeField] private EnemyStats enemyStats;
    
    private GameObject _gameManagerObject;
    private GameManager _gameManager;
    
     

    private void Start()
    {
        _gameManagerObject = GameObject.FindGameObjectWithTag("Manager");
        _gameManager = _gameManagerObject.GetComponent<GameManager>();
        
        GameObject newEnemy = Instantiate(bossPrefab, new Vector3(0, 10, 0), Quaternion.identity);
        enemyStats = newEnemy.GetComponent<EnemyStats>();
        
        enemyStats.OnEnemyDeath += EndWaveOnBossDeath;
    }

    private void OnDestroy()
    {
        enemyStats.OnEnemyDeath -= EndWaveOnBossDeath;
    }

    private void EndWaveOnBossDeath()
    {
        _gameManager.remainingTime = 0;
    }
}
