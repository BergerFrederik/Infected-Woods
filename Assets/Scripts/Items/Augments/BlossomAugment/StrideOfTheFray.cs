using UnityEngine;

public class StrideOfTheFray : MonoBehaviour
{
    [SerializeField] private float moveSpeedGainPerStack;
    [SerializeField] private float attackSpeedConversionRate;

    private Transform PlayerTransform;
    private PlayerStats playerStats;
    private PlayerDealsDamage playerDealsDamage;
    private float currentASModFromMS = 0f;

    public int strideOfTheFrayStacks;

    private void Start()
    {
        PlayerTransform = this.transform.root;
        playerStats = this.GetComponentInParent<PlayerStats>();
        playerDealsDamage = PlayerTransform.GetComponentInChildren<PlayerDealsDamage>();

        playerDealsDamage.OnPlayerHitsEnemyWithWeapon += AddMovespeedToPlayerStats;
        GameManager.OnRoundOver += ResetStacks;
        playerStats.OnMovespeedChanged += RecalculateAttackSpeed;

    }

    private void OnDestroy()
    {
        playerDealsDamage.OnPlayerHitsEnemyWithWeapon -= AddMovespeedToPlayerStats;
        GameManager.OnRoundOver             -= ResetStacks;
        playerStats.OnMovespeedChanged      -= RecalculateAttackSpeed;
    }

    private void AddMovespeedToPlayerStats(WeaponStats weaponStats)
    {
        if (weaponStats.weaponWeaponType != WeaponStats.weaponTypeOptions.Ability)
        {
            strideOfTheFrayStacks++;
            playerStats.playerMovespeed += moveSpeedGainPerStack; 
        }
    }

    private void RecalculateAttackSpeed()
    {       
        playerStats.playerAttackSpeed -= currentASModFromMS;
        float totalCurrentMoveSpeed = playerStats.GetCurrentPlayerMovespeed();
        float newASMod = totalCurrentMoveSpeed * (attackSpeedConversionRate/100);
        playerStats.playerAttackSpeed += newASMod;
        currentASModFromMS = newASMod;
    }

    private void ResetStacks()
    {
        playerStats.playerMovespeed -= moveSpeedGainPerStack * strideOfTheFrayStacks;
        playerStats.playerAttackSpeed -= currentASModFromMS;

        strideOfTheFrayStacks = 0;
        currentASModFromMS = 0f;
    }  
}
