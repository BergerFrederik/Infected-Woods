using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class PlayerGainsHP : MonoBehaviour
{
    [SerializeField] private PlayerStats playerStats;
    [SerializeField] private GameObject popUpPrefab;
    [SerializeField] private float hp_regen_division_const = 10f;
    [SerializeField] private InstantiatePopUp instantiatePopUp;
    
    private float hpAccumulator;

    // Steps that reshape a heal after the heal power is applied (e.g. CriticalHealing). Each one gets the
    // running amount and returns the new one, in the order they were added.
    private readonly List<Func<float, float>> healModifiers = new();

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
                playerStats.playerCurrentHP += CalculateHeal(wholeHPToHeal);
                hpAccumulator -= wholeHPToHeal;
            }
        }
    }

    public void AddHealModifier(Func<float, float> modifier)
    {
        healModifiers.Add(modifier);
    }

    public void RemoveHealModifier(Func<float, float> modifier)
    {
        healModifiers.Remove(modifier);
    }

    // The one place a heal's final amount is decided. Every heal goes through here before it touches HP.
    // Heal power (in percent) comes first, then the modifiers, so they work on the already increased amount.
    public float CalculateHeal(float initialHeal)
    {
        float healAmount = initialHeal + initialHeal * playerStats.playerHealPower / 100f;

        foreach (Func<float, float> modifier in healModifiers)
        {
            healAmount = modifier(healAmount);
        }

        // A heal must never end up hurting, whatever heal power or a modifier does to it
        return Mathf.Max(0f, healAmount);
    }

    // Melee and ranged hits roll lifesteal, mystic and ability hits roll spellvamp instead, effects (e.g. Bleed ticks) never heal.
    // PlayerDealsDamage calls this once per hit, so no weapon or ability has to call it itself.
    // A weapon type that is not listed here heals nothing.
    public void TryApplyLifesteal(WeaponStats weaponStats)
    {
        float cummulatedProbability = weaponStats.weaponWeaponType switch
        {
            WeaponStats.weaponTypeOptions.Melee or WeaponStats.weaponTypeOptions.Ranged
                => playerStats.playerLifeSteal + weaponStats.weaponLifesteal,
            WeaponStats.weaponTypeOptions.Mystic or WeaponStats.weaponTypeOptions.Ability
                => playerStats.playerSpellvamp + weaponStats.weaponSpellvamp,
            _ => 0f
        };
        if (Random.Range(1, 101) <= cummulatedProbability)
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
            playerStats.playerCurrentHP += CalculateHeal(amount);
        }
    }

    private void InstantiatePopUp(float amount)
    {
        instantiatePopUp.Instantiate(amount, false, this.transform);
    }
}
