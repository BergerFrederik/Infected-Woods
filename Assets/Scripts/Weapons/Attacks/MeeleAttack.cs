using System;
using System.Collections;
using UnityEngine;


public class MeeleAttack : MonoBehaviour
{
    [SerializeField] private WeaponStats weaponStats;
    [SerializeField] private float weaponRotationOffsetLeave = 0f;
    [SerializeField] private float weaponRotationOffsetReturn = 0f;
    private enum WeaponState
    {
        Idle,
        Attacking,
        Waiting,
        Returning,
        Cooldown
    }


    private GameObject GameManager;
    private GameManager gameManager;
    private WeaponState currentState = WeaponState.Idle;
    private Transform weaponAnchorPoint;
    private Transform playerTransform;
    private PlayerStats playerStats;
    private Vector3 initialAttackPosition;
    private Vector3 directionToEnemy;
    private BoxCollider2D triggerCollider;

    private float attackCooldown;
    private float cooldownStarttime;


    private void Start()
    {
        cooldownStarttime = 0;
        weaponAnchorPoint = transform.parent;
        triggerCollider = GetComponent<BoxCollider2D>();
        playerTransform = GameObject.FindGameObjectWithTag("Player").transform;
        playerStats = playerTransform.GetComponent<PlayerStats>();
        GameManager = GameObject.FindGameObjectWithTag("Manager");
        gameManager = GameManager.GetComponent<GameManager>();
        gameManager.OnRoundOver += ResetWeaponPosition;
    }

    private void OnDestroy()
    {
        gameManager.OnRoundOver -= ResetWeaponPosition;
    }

    private void Update()
    {
        UpdateCooldown();
        if (currentState == WeaponState.Idle || currentState == WeaponState.Cooldown)
        {
            PointWeaponAtEnemy();
        }
        switch (currentState)
        {
            case WeaponState.Idle:
                if (Time.time - cooldownStarttime >= attackCooldown)
                {
                    SearchForEnemyAndAttack();                    
                }
                break;
            case WeaponState.Attacking:
                MoveToTargetPosition();
                break;
            case WeaponState.Waiting:
                break;
            case WeaponState.Returning:
                ReturnToPlayer();
                break;
            case WeaponState.Cooldown:
                break;
        }
    }

    private void UpdateCooldown()
    {
        float playerAttackSpeed = playerStats.playerAttackSpeed / 100;
        float weaponAttackSpeedCooldown = weaponStats.weaponAttackSpeedCooldown;
        attackCooldown = weaponAttackSpeedCooldown / (1f + playerAttackSpeed);
    }
    private void SearchForEnemyAndAttack()
    {
        float searchRadius = Mathf.Infinity;
        float closestDistance = Mathf.Infinity;
        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, searchRadius);
        Transform closestEnemy = FindClosestEnemy();
        if (closestEnemy != null)
        {
            closestDistance = Vector2.Distance(transform.position, closestEnemy.transform.position);
        }       
        float attackRange = weaponStats.weaponRange + weaponStats.weaponRange * (playerStats.playerAttackRange / 100);

        if (closestEnemy != null && closestDistance <= attackRange)
        {
            currentState = WeaponState.Attacking;
            initialAttackPosition = transform.position;
            transform.parent = null;
            directionToEnemy = (closestEnemy.position - transform.position);
        }
    }

    private void MoveToTargetPosition()
    {
        triggerCollider.enabled = true;
        float projectileSpeed = weaponStats.weaponProjectileSpeed;        
        float weaponRange = weaponStats.weaponRange + weaponStats.weaponRange * (playerStats.playerAttackRange / 100); 
        Vector3 travelDirection = directionToEnemy;
        transform.position += travelDirection.normalized * projectileSpeed * Time.deltaTime;
        float distanceTraveled = Vector2.Distance(initialAttackPosition, transform.position);
        if (distanceTraveled >= weaponRange)
        {
            triggerCollider.enabled = false;
            StartCoroutine(ReturnAfterDelay());
        }
    }

    private IEnumerator ReturnAfterDelay()
    {
        currentState = WeaponState.Waiting;
        float attackDelay = 0.15f;
        yield return new WaitForSeconds(attackDelay);
        currentState = WeaponState.Returning;
    }

    private void ReturnToPlayer()
    {
        float projectileSpeed = weaponStats.weaponProjectileSpeed;
        Vector3 targetPosition = weaponAnchorPoint.position;
        Vector3 targetDirection = (targetPosition - transform.position).normalized;
        transform.position += targetDirection * projectileSpeed * Time.deltaTime;

        float angle = Mathf.Atan2(targetDirection.y, targetDirection.x) * Mathf.Rad2Deg + weaponRotationOffsetReturn;
        transform.rotation = Quaternion.Euler(0, 0, angle);

        if (Vector3.Distance(transform.position, targetPosition) < 1f)
        {
            transform.SetParent(weaponAnchorPoint);
            // Positioniere die Waffe exakt am Ankerpunkt
            transform.localPosition = new Vector3(0, 0, 0);
            cooldownStarttime = Time.time;
            currentState = WeaponState.Idle;
        }           
    }

    private void PointWeaponAtEnemy()
    {
        Transform closestEnemy = FindClosestEnemy();
        if (closestEnemy != null)
        {
            Vector3 directionToEnemy = closestEnemy.position - transform.position;
            float angle = Mathf.Atan2(directionToEnemy.y, directionToEnemy.x) * Mathf.Rad2Deg + weaponRotationOffsetLeave;
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

    private void ResetWeaponPosition()
    {
        if (this.transform.parent == null && currentState != WeaponState.Idle)
        {
            transform.SetParent(weaponAnchorPoint);
            transform.localPosition = new Vector3(0, 0, 0);
            currentState = WeaponState.Idle;
        }
    }
}