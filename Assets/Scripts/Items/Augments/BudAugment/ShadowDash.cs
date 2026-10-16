using UnityEngine;

namespace Items.Augments.BudAugment
{
    public class ShadowDash : MonoBehaviour, IPlayerDependentDescribable
    {
        [SerializeField] private float immunityCooldown; //in seconds

        private Transform _playerTransform;
        private DashAbility _dashAbility;
        private PlayerStats _playerStats;
        private WeaponStats _weaponStats;
        private PlayerDealsDamage _playerDealsDamage;
        private Collider2D _pierceCollider;

        private float _cooldownStartTime = float.NegativeInfinity;
        private bool _isImmune; 

        private void Start()
        {
            _playerTransform = transform.root;
            _dashAbility = _playerTransform.GetComponentInChildren<DashAbility>();
            _playerStats = _playerTransform.GetComponent<PlayerStats>();
            _playerDealsDamage = _playerTransform.GetComponentInChildren<PlayerDealsDamage>();
            _weaponStats = transform.GetComponent<WeaponStats>();
            _pierceCollider = GetComponent<Collider2D>();

            _dashAbility.OnDashUsed += BeginDash;
            _dashAbility.OnDashEnded += FinishDash;
            GameManager.OnRoundOver += CancelDashOnRoundEnd;
        }

        private void OnDestroy()
        {
            _dashAbility.OnDashUsed -= BeginDash;
            _dashAbility.OnDashEnded -= FinishDash;
            GameManager.OnRoundOver -= CancelDashOnRoundEnd;
            ReleaseImmunity();
        }

        private void BeginDash()
        {
            _pierceCollider.enabled = true;
            if (Time.time - _cooldownStartTime > immunityCooldown)
            {
                _isImmune = true;
                _playerStats.playerImmuneToDamageStacks += 1;
            }
        }

        private void FinishDash(float remainingDashCooldown)
        {
            _pierceCollider.enabled = false;
            if (_isImmune)
            {
                ReleaseImmunity();
                _cooldownStartTime = Time.time;
            }
        }

        private void CancelDashOnRoundEnd()
        {
            _pierceCollider.enabled = false;
            ReleaseImmunity();
        }

        private void ReleaseImmunity()
        {
            if (!_isImmune) return;

            _isImmune = false;
            _playerStats.playerImmuneToDamageStacks -= 1;
        }

        private void OnTriggerEnter2D(Collider2D collider)
        {
            if (collider.TryGetComponent<EnemyStats>(out EnemyStats enemyStats))
            {
                _playerDealsDamage.ApplyNonCritableDamageToEnemy(enemyStats, _weaponStats);
            }
        }
        
        public float GetPlaceholderValue(int index)
        {
            return GetPlaceholderValue(index, _playerStats);
        }

        public float GetPlaceholderValue(int index, PlayerStats playerStats)
        {
            // The description is built from the prefab asset, where Start never ran - so read the stats directly
            WeaponStats weaponStats = GetComponent<WeaponStats>();
            float mysticDamage = weaponStats.weaponMysticDamageScale * playerStats.playerMysticDamage / 100f;
            return index switch
            {
                0 => immunityCooldown,
                1 => weaponStats.weaponBaseDamage,
                2 => mysticDamage,
                _ => 0f
            };
        }
    }
}
