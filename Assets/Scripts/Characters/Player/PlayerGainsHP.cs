using System;
using UnityEngine;
using Random = UnityEngine.Random;

public class PlayerGainsHP : MonoBehaviour
{
    [SerializeField] private PlayerStats playerStats;
    [SerializeField] private GameObject popUpPrefab;
    [SerializeField] private float hp_regen_division_const = 10f;
    [SerializeField] private InstantiatePopUp instantiatePopUp;
    
    private float hpAccumulator;

    public event Action<float> OnPlayerWasHealed;

    private void OnEnable()
    {
        playerStats.OnPlayerHealed += InstantiatePopUp;
    }

    private void OnDisable()
    {
        playerStats.OnPlayerHealed -= InstantiatePopUp;
    }

    void Update()
    {
        HealPlayerPerSecond();
    }
    
    private void HealPlayerPerSecond()
    {
        // based on Lifereg stats
        if (playerStats.playerCurrentHP < playerStats.playerMaxHP)
        {
            // "Stat / 10" = HP per sec
            float hpPerSecond = playerStats.playerHPRegeneration / hp_regen_division_const;
            // Negative regen does nothing - it must not build up a debt that blocks regen later
            hpAccumulator += Mathf.Max(0f, hpPerSecond) * Time.deltaTime;

            if (hpAccumulator >= 1f)
            {
                int wholeHPToHeal = Mathf.FloorToInt(hpAccumulator);
                playerStats.playerCurrentHP += wholeHPToHeal;
                hpAccumulator -= wholeHPToHeal;
            }
            OnPlayerWasHealed?.Invoke(playerStats.playerCurrentHP);
        }
    }

    public void TryApplyLifesteal(EnemyStats enemyStats, WeaponStats weaponStats)
    {
        float cummulatedLifestealProbability = playerStats.playerLifeSteal + weaponStats.weaponLifesteal;
        if (Random.Range(1, 101) <= cummulatedLifestealProbability)
        {
            ApplyLifestealHeal(1f);
        }
    }

    // Heals without its own chance roll, but only if the player isn't full and the lifesteal
    // cooldown is over. For effects that roll their chance themselves.
    public void ApplyLifestealHeal(float amount)
    {
        bool playerIsNotFullHP = false;
        if (playerStats.playerCurrentHP < playerStats.playerMaxHP)
        {
            playerIsNotFullHP = true;
        }

        bool playerCanLifesteal = false;
        if (Time.time - playerStats.playerLastLifesteal >= 0.1f)
        {
            playerCanLifesteal = true;
        }

        if (playerIsNotFullHP && playerCanLifesteal)
        {
            playerStats.playerLastLifesteal = Time.time;
            playerStats.playerCurrentHP += amount;
            OnPlayerWasHealed?.Invoke(playerStats.playerCurrentHP);
        }
    }

    private void InstantiatePopUp(float amount)
    {
        instantiatePopUp.Instantiate(amount, false, this.transform);
    }
}
