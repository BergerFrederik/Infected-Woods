using System.Collections;
using UnityEngine;

public class Ranged : MonoBehaviour
{
    [SerializeField] private float weaponRotationOffset = 0f;
    [SerializeField] private float projectileRotationOffset = 0f;
    [SerializeField] private WeaponStats weaponStats;
    [SerializeField] private GameObject WeaponProjectile;

    private Transform PlayerTransform;
    private WeaponState currentWeaponState = WeaponState.Idle;
    private Vector3 initialAttackPosition;
    private Vector3 directionToEnemy;

    public float attackCooldown = 0f;

    private enum WeaponState
    {
        Idle,
        Shoot,
        Cooldown
    }

    private void Start()
    {
        PlayerTransform = GameObject.FindGameObjectWithTag("Player").transform;
    }


    private void Update()
    {
        if (currentWeaponState == WeaponState.Idle || currentWeaponState == WeaponState.Cooldown)
        {
            PointWeaponAtEnemy();
        }
        switch (currentWeaponState)
        {
            case WeaponState.Idle:
                SearchForEnemyAndAttack();
                break;
            case WeaponState.Shoot:
                ShootEnemy();
                break;
            case WeaponState.Cooldown:
                break;
        }
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
            initialAttackPosition = transform.position;
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

        projectileScript.sourceWeaponStats = this.weaponStats;

        ApplyCooldown();
    }

    private void ApplyCooldown()
    {
        StartCoroutine(WaitCooldown());
    }

    private float player_Attackspeed_division_const = 100f;
    private IEnumerator WaitCooldown()
    {
        currentWeaponState = WeaponState.Cooldown;
        float cooldown = attackCooldown;
        float playerAttackSpeed = PlayerTransform.GetComponent<PlayerStats>().playerAttackSpeed / player_Attackspeed_division_const;
        if (playerAttackSpeed > 0)
        {
            cooldown /= (1 + playerAttackSpeed);
        }
        yield return new WaitForSeconds(cooldown);
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