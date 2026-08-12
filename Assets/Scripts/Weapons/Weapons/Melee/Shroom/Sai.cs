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
    [SerializeField] private float[] dotTimePerLevel;
    
    private RandomRollEvent _randomRollEvent;
    private Transform _playerTransform;
    

    private void OnTransformParentChanged()
    {
        Transform rootTransform = transform.root;
        if (rootTransform.name == "Player")
        {
            _playerTransform = rootTransform;
            _randomRollEvent = _playerTransform.GetComponentInChildren<RandomRollEvent>();
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
        int weaponLevel = (int)weaponStats.WeaponLevel;
        if (randomRoll >= 100 - (int)chanceToApplyBleedPerLevel[weaponLevel])
        {
            GameObject dot = Instantiate(bleedEffectPrefab, enemyTransform);
            DamageOverTime damageOverTime = dot.GetComponent<DamageOverTime>();
            damageOverTime.dotTime = dotTimePerLevel[weaponLevel];
            damageOverTime.playerDealsDamage = _playerTransform.GetComponentInChildren<PlayerDealsDamage>();
            damageOverTime.enemyStats = enemyStats;
        }
    }
}
