using UnityEngine;


public class Player : MonoBehaviour
{
    [SerializeField] private PlayerStats playerStats;
    [SerializeField] private DropsEvent dropsEvent;
    [SerializeField] private PlayerTakesDamage playerTakesDamage;

    public HealthBar healthBar;
    public LevelBar levelBar;

    private void Start()
    {
        playerStats.playerCurrentHP = playerStats.playerMaxHP;
        healthBar.SetMaxHealth(playerStats.playerMaxHP);
    }

    private void Update()
    {       
        HealPlayerPerSecond();
        UpdateHealthbar();
        UpdateXPBar();
    }

    private float hpAccumulator;
    private float hp_regen_division_const = 10f;
    private void HealPlayerPerSecond()
    {
        // based on Lifereg stats
        if (playerStats.playerCurrentHP < playerStats.playerMaxHP)
        {
            // "Stat / 10" = HP per sec
            float hpPerSecond = playerStats.playerHPRegeneration / hp_regen_division_const;
            hpAccumulator += hpPerSecond * Time.deltaTime;

            if (hpAccumulator >= 1f)
            {
                int wholeHPToHeal = Mathf.FloorToInt(hpAccumulator);
                playerStats.playerCurrentHP += wholeHPToHeal;
                hpAccumulator -= wholeHPToHeal;
            }
        }
    }

    private float oldCurrentPlayerHealth = 0f;

    private void UpdateHealthbar()
    {
        float newCurrentPlayerHealth = playerStats.playerCurrentHP;
        if (newCurrentPlayerHealth != oldCurrentPlayerHealth)
        {
            healthBar.SetHealth(newCurrentPlayerHealth);
            oldCurrentPlayerHealth = newCurrentPlayerHealth;
        }
    }

    private float GetRequiredXPForNextLevel()
    {
        float requiredXP = playerStats.playerBaseXP * Mathf.Pow(playerStats.playerLevelMultiplier, playerStats.playerLevel - 1);
        return requiredXP;
    }

    private float oldCurrentPlayerXP = 0f;

    private void UpdateXPBar()
    {
        float newCurrentPlayerXP = playerStats.playerCurrentXP;
        float requiredXP = GetRequiredXPForNextLevel();
        if (playerStats.playerCurrentXP >= requiredXP)
        {
            playerStats.playerLevel++;
            playerStats.playerCurrentXP -= requiredXP;
            oldCurrentPlayerXP = playerStats.playerCurrentXP;
            levelBar.SetExpNeeded(GetRequiredXPForNextLevel(), playerStats.playerCurrentXP);
        }
        else if (newCurrentPlayerXP > oldCurrentPlayerXP)
        {
            levelBar.SetExp(newCurrentPlayerXP);
            oldCurrentPlayerXP = newCurrentPlayerXP;
        }
    }

    // player should get constantly damage while touching an enemy

    private float iFrameStartTime = 0f;
    private float currentIFrames = 0f;
    private void OnTriggerStay2D(Collider2D collider)
    {
        if (Time.time - iFrameStartTime >= currentIFrames)
        {
            if (collider.TryGetComponent<EnemyStats>(out EnemyStats enemyStats))
            {
                float damageDealtToPlayer = playerTakesDamage.DealDamageToPlayer(enemyStats);
                currentIFrames = SetIFrames(damageDealtToPlayer);
                iFrameStartTime = Time.time;
            }
        }      
    }

    private float iframe_formula_const_1 = 0.4f;
    private float iframe_formula_const_2 = 0.15f;
    private float SetIFrames(float damageDealtToPlayer)
    {
        float playerMaxHP = playerStats.playerMaxHP;
        float iframes = iframe_formula_const_1 * ((damageDealtToPlayer / playerMaxHP) / iframe_formula_const_2);
        iframes = Mathf.Clamp(iframes, 0.2f, 0.4f);
        return iframes;
    }

    private void OnTriggerEnter2D(Collider2D collider)
    {
        dropsEvent.GainMoney(collider);
    }  
}
