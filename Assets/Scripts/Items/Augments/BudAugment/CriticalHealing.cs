using UnityEngine;

namespace Items.Augments.BudAugment
{
    // Heals can crit: with the player's crit chance a heal is multiplied.
    // Hooks into PlayerGainsHP.CalculateHeal as a heal modifier, which runs after the heal power is applied,
    // so the whole increased amount can crit.
    public class CriticalHealing : MonoBehaviour, IAugmentDescribable
    {
        [SerializeField] private float exponentialPowerOfFactorIncrease = 0.8f;
        private PlayerStats _playerStats;
        private PlayerGainsHP _playerGainsHp;
        
        private void Start()
        {
            _playerStats = transform.root.GetComponent<PlayerStats>();
            _playerGainsHp = transform.root.GetComponentInChildren<PlayerGainsHP>();

            _playerGainsHp.AddHealModifier(ModifyHeal);
        }

        private void OnDestroy()
        {
            if (_playerGainsHp != null)
            {
                _playerGainsHp.RemoveHealModifier(ModifyHeal);
            }
        }

        // Always returns the heal: unchanged without a crit, multiplied with one
        private float ModifyHeal(float healAmount)
        {
            bool isCrit = Random.Range(0f, 100f) < _playerStats.PlayerCritChance;
            return isCrit ? healAmount * GetCritHealMultiplier() : healAmount;
        }

        // 1x plus 1 per 100 heal power, plus a bonus that grows faster than heal power itself
        // (+0.1 at 100, about +1.5 at 1000), so every extra point of heal power is worth more than the last.
        private float GetCritHealMultiplier()
        {
            if (_playerStats == null)
            {
                return 1f;
            }

            // Negative heal power would make the power curve below NaN
            float healPower = Mathf.Max(0f, _playerStats.playerHealPower);
            float bonus = 0.1f * Mathf.Pow(healPower / 100f, exponentialPowerOfFactorIncrease);
            return 1f + healPower / 100f + bonus;
        }

        public float GetPlaceholderValue(int index)
        {
            return index switch
            {
                0 => _playerStats != null ? _playerStats.PlayerCritChance : 0f,
                1 => GetCritHealMultiplier(),
                _ => 0f
            };
        }
    }
}
