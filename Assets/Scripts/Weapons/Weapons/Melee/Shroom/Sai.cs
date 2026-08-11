using System;
using UnityEngine;

public class Sai : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private MeleeWeaponHitsEnemy meleeWeaponHitsEnemy;
    [SerializeField] private GameObject bleedEffectPrefab;
    [SerializeField] private WeaponStats weaponStats;

    [Header("Values")]
    [SerializeField] private float[] chanceToApplyBleedPerLevel;
    
    private RandomRollEvent _randomRollEvent;
    

    private void OnTransformParentChanged()
    {
        if (transform.root.name == "Player")
        {
            _randomRollEvent = transform.root.GetComponentInChildren<RandomRollEvent>();
        }
    }

    private void OnEnable()
    {
        meleeWeaponHitsEnemy.OnMeleeWeaponHitsEnemy += ApplyBleed;
    }

    private void OnDisable()
    {
        meleeWeaponHitsEnemy.OnMeleeWeaponHitsEnemy -= ApplyBleed;
    }

    private void ApplyBleed(EnemyStats enemyStats, bool didCrit)
    {
        if (!didCrit) return;
        
        Transform enemyTransform = enemyStats.transform;
        if (enemyTransform.GetComponentInChildren<DamageOverTime>() != null) return;

        int randomRoll = _randomRollEvent.GetRandomIntRoll(0, 100);
        if (randomRoll >= 100 - (int)chanceToApplyBleedPerLevel[(int)weaponStats.WeaponLevel])
        {
            Instantiate(bleedEffectPrefab, enemyTransform);
        }
    }
}
