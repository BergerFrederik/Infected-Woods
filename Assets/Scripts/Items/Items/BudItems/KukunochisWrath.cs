using UnityEngine;

public class KukunochisWrath : MonoBehaviour
{
    [SerializeField] private ItemInformation itemInformation;
    [SerializeField] private float critChanceThreshold = 200f;
    private PlayerStats _playerStats;
    
    
    private bool _isThresholdReached;

    private void OnEnable()
    {
        if (itemInformation.IsPlayerRoot())
        {
            _playerStats = this.transform.root.GetComponent<PlayerStats>();
            _playerStats.OnCritChanceChanged += DoubleItemCritDamage;
            DoubleItemCritDamage(_playerStats.PlayerCritChance);
        }
    }

    private void OnDisable()
    {
        if (itemInformation.IsPlayerRoot())
        {
            _playerStats.OnCritChanceChanged -= DoubleItemCritDamage;
        }
    }

    private void DoubleItemCritDamage(float critChance)
    {
        if (_playerStats.PlayerCritChance >= critChanceThreshold && !_isThresholdReached)
        {
            HandleItemCritDamage(true);
            _isThresholdReached = true;
        }

        if (_playerStats.PlayerCritChance < critChanceThreshold && _isThresholdReached)
        {
            HandleItemCritDamage(false);
            _isThresholdReached = false;
        }
    }

    private void HandleItemCritDamage(bool isAdding)
    {
        foreach (ItemStat stat in itemInformation.stats)
        {
            if (stat.statType == StatType.CritDamage)
            {
                if (isAdding)
                    _playerStats.PlayerCritDamage += stat.value;
                else
                    _playerStats.PlayerCritDamage -= stat.value;
            }
        }
    }
}
