using System;
using UnityEngine;

namespace Items.Augments.BlossomAugment
{
    // Health and Mana feed each other: every point of Health gained also gives Mana, and every point of Mana
    // gained also gives Health. Losses and spending are not passed on, only gains (the events fire for nothing else).
    // Max Health and Max Mana follow each other the same way, in both directions: a raise is passed on, and a
    // drop (e.g. selling an item) takes back what this augment gave, never more.
    public class YinYang : MonoBehaviour
    {
        private PlayerStats _playerStats;
        private PlayerGainsHP _playerGainsHp;

        private bool _isTransferring;
        
        private float _maxHpGiven;
        private float _maxMpGiven;

        private void Start()
        {
            _playerStats = transform.root.GetComponent<PlayerStats>();
            _playerGainsHp = transform.root.GetComponentInChildren<PlayerGainsHP>();

            _playerStats.OnPlayerHealed += GiveManaForHeal;
            _playerStats.OnPlayerInfused += GiveHealthForMana;
            _playerStats.OnMaxHPIncreased += GiveMaxManaForHealth;
            _playerStats.OnMaxMPIncreased += GiveMaxHealthForMana;
            _playerStats.OnMaxHPDecreased += TakeMaxManaForHealth;
            _playerStats.OnMaxMPDecreased += TakeMaxHealthForMana;
        }

        private void OnDestroy()
        {
            if (_playerStats == null) return;

            _playerStats.OnPlayerHealed -= GiveManaForHeal;
            _playerStats.OnPlayerInfused -= GiveHealthForMana;
            _playerStats.OnMaxHPIncreased -= GiveMaxManaForHealth;
            _playerStats.OnMaxMPIncreased -= GiveMaxHealthForMana;
            _playerStats.OnMaxHPDecreased -= TakeMaxManaForHealth;
            _playerStats.OnMaxMPDecreased -= TakeMaxHealthForMana;
        }
        
        private void Transfer(Action transfer)
        {
            if (_isTransferring) return;

            _isTransferring = true;
            try
            {
                transfer();
            }
            finally
            {
                _isTransferring = false;
            }
        }

        private void GiveManaForHeal(float healed)
        {
            Transfer(() => _playerStats.playerCurrentMP += healed);
        }

        private void GiveHealthForMana(float manaGained)
        {
            Transfer(() => _playerStats.playerCurrentHP += _playerGainsHp.CalculateHeal(manaGained));
        }

        private void GiveMaxHealthForMana(float amount)
        {
            Transfer(() =>
            {
                _playerStats.playerMaxHP += amount;
                _maxHpGiven += amount;
            });
        }

        private void GiveMaxManaForHealth(float amount)
        {
            Transfer(() =>
            {
                _playerStats.playerMaxMP += amount;
                _maxMpGiven += amount;
            });
        }

        private void TakeMaxHealthForMana(float amount)
        {
            Transfer(() =>
            {
                float taken = Mathf.Min(amount, _maxHpGiven);
                _playerStats.playerMaxHP -= taken;
                _maxHpGiven -= taken;
            });
        }

        private void TakeMaxManaForHealth(float amount)
        {
            Transfer(() =>
            {
                float taken = Mathf.Min(amount, _maxMpGiven);
                _playerStats.playerMaxMP -= taken;
                _maxMpGiven -= taken;
            });
        }
    }
}
