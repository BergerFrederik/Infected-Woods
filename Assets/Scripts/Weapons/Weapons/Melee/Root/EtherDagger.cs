using UnityEngine;

public class EtherDagger : MonoBehaviour
{
    [SerializeField] private float[] maxManaDrainPerLevel;
    [SerializeField] private MeleeWeaponHitsEnemy meleeWeaponHitsEnemy;
    [SerializeField] private WeaponStats weaponStats;

    private PlayerStats _playerStats;

    private void Start()
    {
        meleeWeaponHitsEnemy.OnMeleeWeaponHitsEnemy += DrainManaOnHit;
    }

    private void OnDestroy()
    {
        meleeWeaponHitsEnemy.OnMeleeWeaponHitsEnemy -= DrainManaOnHit;
    }

    private void DrainManaOnHit(EnemyStats enemyStats, bool isCrit)
    {
        if (_playerStats == null) _playerStats = meleeWeaponHitsEnemy.Owner.GetComponent<PlayerStats>();
        float weaponLevel = weaponStats.WeaponLevel;
        int manaGained = Random.Range(1, (int)maxManaDrainPerLevel[(int)weaponLevel] + 1);
        if (isCrit) manaGained *= 2;
        _playerStats.playerCurrentMP += manaGained;
    }
}
