using UnityEngine;

// Chance to gain Light on critical hits.
// Every crit that damages an enemy counts, whatever its source (weapon, projectile, ability).

public class CriticalLight : MonoBehaviour, IAugmentDescribable
{
    [SerializeField] private float lightPerCrit;
    [SerializeField] private float chanceForLight;

    private Transform _player;
    private PlayerStats _playerStats;
    private PlayerDealsDamage _playerDealsDamage;
    private RandomRollEvent _randomRollEvent;

    private void Start()
    {
        _player = this.transform.root;
        _playerStats = _player.GetComponent<PlayerStats>();
        _playerDealsDamage = _player.GetComponentInChildren<PlayerDealsDamage>();
        _randomRollEvent = _player.GetComponentInChildren<RandomRollEvent>();

        _playerDealsDamage.OnPlayerCritsEnemy += PerformAugment;
    }

    private void OnDestroy()
    {
        if (_playerDealsDamage != null)
        {
            _playerDealsDamage.OnPlayerCritsEnemy -= PerformAugment;
        }
    }

    private void PerformAugment(EnemyStats enemyStats, WeaponStats weaponStats)
    {
        if (_randomRollEvent.GetRandomFloatRoll(0f, 100f) > 100f - chanceForLight)
        {
            _playerStats.PlayerLightAmount += lightPerCrit;
        }
    }

    public float GetPlaceholderValue(int index)
    {
        return index switch
        {
            0 => chanceForLight,
            1 => lightPerCrit,
            _ => 0f
        };
    }
}
