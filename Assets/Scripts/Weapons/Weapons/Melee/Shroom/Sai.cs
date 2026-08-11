using UnityEngine;

public class Sai : MonoBehaviour
{
    [SerializeField] private PlayerStats playerStats;
    [SerializeField] private RandomRollEvent randomRollEvent;
    [SerializeField] private MeleeWeaponHitsEnemy meleeWeaponHitsEnemy;

    [SerializeField] private float chanceToApplyBleed;

    private void OnEnable()
    {
        meleeWeaponHitsEnemy.OnMeleeWeaponHitsEnemy += ApplyBleed;
    }

    private void OnDisable()
    {
        meleeWeaponHitsEnemy.OnMeleeWeaponHitsEnemy -= ApplyBleed;
    }

    private void ApplyBleed()
    {
        int randomRoll = randomRollEvent.GetRandomIntRoll(0, 100);
        //check if it was a crit
        if (randomRoll >= chanceToApplyBleed)
        {
            //applyBleed
        }
    }
}
