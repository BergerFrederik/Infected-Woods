using UnityEngine;

public class GlasTree : MonoBehaviour, IAugmentDescribable
{
    [SerializeField] private float damageBonus;
    [SerializeField] private float armorFreezeAt;
    
    private Transform _player;
    private PlayerStats _playerStats;
    private float _armorOverflow;
    private bool _isAdjusting;

    private void Start()
    {
        _player = this.transform.root;
        _playerStats = _player.GetComponent<PlayerStats>();

        ArmorFreeze(_playerStats.PlayerArmor);

        _playerStats.OnPlayerArmorChanged += ArmorFreeze;
        
        PerformAugment();
    }

    private void OnDestroy()
    {
        _playerStats.OnPlayerArmorChanged -= ArmorFreeze;
        _playerStats.PlayerArmor += _armorOverflow;
        _playerStats.playerDamage -= damageBonus;
    }

    private void PerformAugment()
    {
        _playerStats.playerDamage += damageBonus;
    }

    private void ArmorFreeze(float value)
    {
        if (_isAdjusting) return;

        float realArmor = value + _armorOverflow;
        float clamped = Mathf.Min(realArmor, armorFreezeAt);
        _armorOverflow = Mathf.Max(0f, realArmor - armorFreezeAt);

        if (!Mathf.Approximately(clamped, value))
        {
            _isAdjusting = true;
            _playerStats.PlayerArmor = clamped;
            _isAdjusting = false;
        }
    }
    
    public float GetPlaceholderValue(int index)
    {
        return index switch
        {
            0 => damageBonus,
            1 => armorFreezeAt,
            _ => 0f
        };
    }
}
