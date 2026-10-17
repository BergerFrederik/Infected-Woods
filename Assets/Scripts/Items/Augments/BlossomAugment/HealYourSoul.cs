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

        private void Start()
        {
            _playerTransform = transform.root;
            _playerStats = _playerTransform.GetComponent<PlayerStats>();

            _playerStats.OnPlayerHealed += PerformAugment;
        }

        private void OnDestroy()
        {
            _playerStats.OnPlayerHealed -= PerformAugment;
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