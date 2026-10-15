using UnityEngine;

namespace Items.Augments.BudAugment
{
    // Heals can crit: with the player's crit chance a heal is multiplied.
    // Hooks into PlayerGainsHP.CalculateHeal as a heal modifier, which runs after the heal power is applied,
    // so the whole increased amount can crit.
    public class CriticalHealing : MonoBehaviour, IPlayerDependentDescribable
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
            return isCrit ? healAmount * GetCritHealMultiplier(_playerStats) : healAmount;
        }

        // 1.5x plus 1 per 100 heal power, plus a bonus that grows faster than heal power itself
        // (+0.1 at 100, about +1.5 at 1000), so every extra point of heal power is worth more than the last.
        // Takes the player as a parameter because the description is built from the prefab (see IPlayerDependentDescribable)
        private float GetCritHealMultiplier(PlayerStats playerStats)
        {
            if (playerStats == null)
            {
                return 1.5f;
            }

            // Negative heal power would make the power curve below NaN
            float healPower = Mathf.Max(0f, playerStats.playerHealPower);
            float bonus = 0.1f * Mathf.Pow(healPower / 100f, exponentialPowerOfFactorIncrease);
            return 1.5f + healPower / 100f + bonus;
        }

        // Only used when nobody hands in a player; the description builder prefers the overload below
        public float GetPlaceholderValue(int index)
        {
            return GetPlaceholderValue(index, _playerStats);
        }

        public float GetPlaceholderValue(int index, PlayerStats playerStats)
        {
            return index switch
            {
                0 => playerStats != null ? playerStats.PlayerCritChance : 0f,
                1 => GetCritHealMultiplier(playerStats),
                _ => 0f
            };
        }
    }
}
