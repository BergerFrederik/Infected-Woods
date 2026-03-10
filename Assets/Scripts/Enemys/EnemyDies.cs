using UnityEngine;

public class EnemyDies : MonoBehaviour
{
    [SerializeField] private EnemyStats enemyStats;
    [SerializeField] private GameObject lightDropPrefab;
    [SerializeField] private EnemyIsHitByPlayer enemyIsHitByPlayer;

    private GameObject Player;
    private PlayerGainsEXP playerGainsEXP;
    
    private EntitySpawner _entitySpawner;

    public void SetupSpawner(EntitySpawner entitySpawner)
    {
        _entitySpawner = entitySpawner;
    }
    
    private void Start()
    {
        Player = GameObject.FindGameObjectWithTag("Player");
        Transform playerEventManager = Player.transform.Find("PlayerEventManager");
        Transform playerGainsEXPChild = playerEventManager.transform.Find("PlayerGainsEXP");
        playerGainsEXP = playerGainsEXPChild.GetComponent<PlayerGainsEXP>();
        enemyStats.OnEnemyDeath += OnEnemyDeath;
    }

    private void OnDestroy()
    {
        enemyStats.OnEnemyDeath -= OnEnemyDeath;
    }

    private void OnEnemyDeath()
    {
        _entitySpawner.UnregisterEnemy(this.gameObject);
        
        InstantiateDropsOnDeath();
        GiveEXP();
        KillEnemy();
    }

    private void KillEnemy()
    {
        Destroy(this.gameObject);
    }
    private void InstantiateDropsOnDeath()
    {
        GameObject newLightDrop = Instantiate(lightDropPrefab, transform.position, Quaternion.identity);
        if (newLightDrop.TryGetComponent<LightDrop>(out LightDrop lightDrop))
        {
            lightDrop.lightDropValue = enemyStats.enemyLightDropped;
        }
    }

    private void GiveEXP()
    {
        playerGainsEXP.GiveXPByEnemyToPlayer(enemyStats);
    }
}
