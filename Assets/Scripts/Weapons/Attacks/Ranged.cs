using System.Collections;
using UnityEngine;

public class Ranged : MonoBehaviour
{
    [SerializeField] private float weaponRotationOffset = 0f;
    [SerializeField] private float projectileRotationOffset = 0f;
    [SerializeField] private WeaponStats weaponStats;
    [SerializeField] private GameObject WeaponProjectile;

    private Transform PlayerTransform;
    private PlayerStats playerStats;
    private WeaponState currentWeaponState = WeaponState.Idle;
    private Vector3 directionToEnemy;

    private float attackCooldown;
    private float cooldownStarttime;

    private enum WeaponState
    {
        Idle,
        Shoot
    }

    private void Start()
    {
        PlayerTransform = GameObject.FindGameObjectWithTag("Player").transform;
        playerStats = PlayerTransform.GetComponent<PlayerStats>();
    }


    private void Update()
    {
        UpdateCooldown();
        if (currentWeaponState == WeaponState.Idle)
        {
            PointWeaponAtEnemy();
        }
        switch (currentWeaponState)
        {
            case WeaponState.Idle:
                if (Time.time - cooldownStarttime >= attackCooldown)
                {
                    SearchForEnemyAndAttack();
                }
                break;
            case WeaponState.Shoot:
                ShootEnemy();
                break;
        }
    }

    private void UpdateCooldown()
    {
        float playerAttackSpeedBonus = playerStats.playerAttackSpeed / 100;
        float weaponAttackSpeedCooldown = weaponStats.weaponAttackSpeedCooldown;
        attackCooldown = weaponAttackSpeedCooldown / (1f + playerAttackSpeedBonus);
    }
    private void SearchForEnemyAndAttack()
    {
        float closestDistance = Mathf.Infinity;
        Transform closestEnemy = FindClosestEnemy();
        if (closestEnemy != null)
        {
            closestDistance = Vector2.Distance(transform.position, closestEnemy.transform.position);
        }
        float attackRange = weaponStats.weaponRange + weaponStats.weaponRange * (PlayerTransform.GetComponent<PlayerStats>().playerAttackRange / 100);

        if (closestEnemy != null && closestDistance <= attackRange)
        {           
            directionToEnemy = (closestEnemy.position - transform.position);
            currentWeaponState = WeaponState.Shoot;
        }
    }

    private void ShootEnemy()
    {
        float angleRadians = Mathf.Atan2(directionToEnemy.y, directionToEnemy.x);
        float angleDegrees = angleRadians * Mathf.Rad2Deg;
        Quaternion targetRotation = Quaternion.Euler(0, 0, (angleDegrees + projectileRotationOffset));

        GameObject newProjectile = Instantiate(WeaponProjectile, transform.position, targetRotation);
        Projectile projectileScript = newProjectile.GetComponent<Projectile>();
        cooldownStarttime = Time.time;

        projectileScript.sourceWeaponStats = this.weaponStats;
        currentWeaponState = WeaponState.Idle;
    }

    private void PointWeaponAtEnemy()
    {
        Transform closestEnemy = FindClosestEnemy();
        if (closestEnemy != null)
        {
            directionToEnemy = closestEnemy.position - transform.position;
            float angle = Mathf.Atan2(directionToEnemy.y, directionToEnemy.x) * Mathf.Rad2Deg + weaponRotationOffset;
            transform.rotation = Quaternion.Euler(0, 0, angle);
        }
    }

    private Transform FindClosestEnemy()
    {
        float searchRadius = Mathf.Infinity;
        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, searchRadius);
        float closestDistance = Mathf.Infinity;
        Transform closestEnemy = null;
        float weaponDistanceToEnemy = 0f;
        foreach (Collider2D collider in colliders)
        {
            if (collider.CompareTag("Enemy"))
            {
                weaponDistanceToEnemy = Vector2.Distance(transform.position, collider.transform.position);

                if (weaponDistanceToEnemy < closestDistance)
                {
                    closestDistance = weaponDistanceToEnemy;
                    closestEnemy = collider.transform;
                }
            }
        }
        return closestEnemy;
    }
}