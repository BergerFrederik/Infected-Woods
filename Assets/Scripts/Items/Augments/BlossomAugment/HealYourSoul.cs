using System.Collections.Generic;
using UnityEngine;

namespace Items.Augments.BlossomAugment
{
    public class HealYourSoul : MonoBehaviour
    {
        [SerializeField] private WeaponStats weaponStats;
        [SerializeField] private GameObject projectilePrefab;
        
        private Transform _playerTransform;
        private PlayerStats _playerStats;
        private float _pendingHealValue;

        private void Start()
        {
            _playerTransform = transform.root;
            _playerStats = _playerTransform.GetComponent<PlayerStats>();

            _playerStats.OnPlayerHealed += CollectHeal;
        }

        private void OnDestroy()
        {
            if (_playerStats == null) return;

            _playerStats.OnPlayerHealed -= CollectHeal;
        }

        // Heals of one frame (e.g. Yin Yang next to a regen tick) are fired as one volley: a projectile copies its damage
        // from weaponStats a frame after it was spawned, so two volleys in one frame would overwrite each other's damage
        private void CollectHeal(float healValue)
        {
            _pendingHealValue += healValue;
        }

        private void LateUpdate()
        {
            if (_pendingHealValue <= 0f) return;

            float healValue = _pendingHealValue;
            _pendingHealValue = 0f;
            PerformAugment(healValue);
        }

        private void PerformAugment(float healValue)
        {
            weaponStats.weaponBaseDamage = healValue;
            List<Transform> enemysInRadius = FindEnemysInRadius();

            foreach (Transform enemy in enemysInRadius)
            {
                GameObject newProjectile = Instantiate(projectilePrefab, transform.position, Quaternion.identity);
                Projectile projectile = newProjectile.GetComponent<Projectile>();
                projectile.sourceWeaponStats = weaponStats;
                projectile.target = enemy;
                newProjectile.GetComponent<ProjectileHitsEnemy>().SetOwner(transform.root);
            }
        }
        
        private List<Transform> FindEnemysInRadius()
        {
            Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, weaponStats.weaponRange);
            List<Transform> enemysInRadius = new();

            foreach (var collider in colliders)
            {
                if (collider.CompareTag("Enemy"))
                {
                    enemysInRadius.Add(collider.transform);
                }
            }
            return enemysInRadius;
        }
    }
}