using UnityEngine;

public class EnemyDies : MonoBehaviour
{
    [SerializeField] private EnemyStats enemyStats;
    [SerializeField] private GameObject lightDropPrefab;
    [SerializeField] private EnemyIsHitByPlayer enemyIsHitByPlayer;

    private GameObject Player;
    private PlayerGainsEXP playerGainsEXP;
    
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
        KillEnemy();
        InstantiateDropsOnDeath();
        GiveEXP();
    }

    private void KillEnemy()
    {
        Destroy(this.gameObject);
    }
    private void InstantiateDropsOnDeath()
    {
        if (Random.Range(0f, 100f) <= enemyStats.enemyLightDropChance)
        {
            GameObject newLightDrop = Instantiate(lightDropPrefab, transform.position, Quaternion.identity);
            if (newLightDrop.TryGetComponent<LightDrop>(out LightDrop lightDrop))
            {
                lightDrop.lightDropValue = enemyStats.enemyLightDropped;
            }
        }
    }

    private void GiveEXP()
    {
        playerGainsEXP.GiveXPByEnemyToPlayer(enemyStats);
    }
}
