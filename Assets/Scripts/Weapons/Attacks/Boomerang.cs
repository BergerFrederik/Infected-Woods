using System.Collections;
using UnityEngine;


public class Boomerang : MonoBehaviour
{
    [Header("Weapon Settings")]
    [SerializeField] private WeaponStats weaponStats;
    [SerializeField] private float weaponRotationOffset = 0f;
    
    [Header("Attack Timings")]
    [SerializeField] private float recoilDuration = 0.1f;
    [SerializeField] private float recoilSpeed = 4f;
    [SerializeField] private float postAttackWaitDelay = 0.15f;

    [Header("Attack Distances")]
    [SerializeField] private float enemySearchRadius = 20f;


    private enum WeaponState { Idle, Attacking }

    private WeaponState currentState = WeaponState.Idle;
    
    private PlayerStats playerStats;
    private Collider2D triggerCollider;

    private float lastAttackTime;
    private float weaponLengthOffset;
    private Transform weaponSocket;

    private void Awake()
    {
        triggerCollider = this.gameObject.GetComponent<Collider2D>();
        weaponSocket = transform.parent;
    }

    private void OnEnable()
    {
        ResetWeaponPosition();
    }

    private void OnDisable()
    {
        ResetWeaponPosition();
    }

    private void Start()
    {
        GameManager.OnRoundOver += ResetWeaponPosition;
        
        if (triggerCollider is BoxCollider2D box)
        {
            weaponLengthOffset = Mathf.Abs(box.offset.x) + (box.size.x / 2f);
        }
        else if (triggerCollider is PolygonCollider2D poly)
        {
            float maxX = 0;
            foreach (Vector2 point in poly.points)
            {
                if (point.x > maxX) maxX = point.x;
            }
            weaponLengthOffset = maxX;
        }
        else
        {
            weaponLengthOffset = 1.0f;
        }
    }

    private void OnDestroy()
    {
        GameManager.OnRoundOver -= ResetWeaponPosition;
    }

    private void Update()
    {
        if (playerStats == null)
        {
            playerStats = transform.root.GetComponentInChildren<PlayerStats>();
            if (playerStats == null) return;
        }

        if (currentState == WeaponState.Idle)
        {
            Transform closestEnemy = FindClosestEnemy();
            PointWeaponAtEnemy(closestEnemy);
            
            float attackCooldown = weaponStats.weaponAttackSpeedCooldown / (1f + playerStats.playerAttackSpeed / 100f);
            attackCooldown = Mathf.Max(attackCooldown, 0.05f);

            if (Time.time - lastAttackTime >= attackCooldown)
            {
                CheckForAttack(closestEnemy);
            }
        }
    }
    
    private void CheckForAttack(Transform closestEnemy)
    {
        if (closestEnemy == null) return;
        Collider2D enemyCollider = closestEnemy.GetComponent<Collider2D>();
        Vector2 closestPointOnEdge = enemyCollider.ClosestPoint(transform.position);
        float distanceToEdge = Vector2.Distance(transform.position, closestPointOnEdge);
        float attackRange = weaponStats.weaponRange + weaponStats.weaponRange * (playerStats.playerAttackRange / 100f);
        
        if (distanceToEdge <= weaponLengthOffset + attackRange)
        {
            StartCoroutine(BoomerangFlyRoutine(closestEnemy, attackRange));
        }
    }
    
    
    private IEnumerator BoomerangFlyRoutine(Transform targetEnemy, float range)
    {
        Vector3 targetPos = targetEnemy.position;
        currentState = WeaponState.Attacking;

        transform.SetParent(null, true);

        // Richtung beim Start des Angriffs fixieren

        Vector3 worldStartPos = transform.position;
        Vector3 attackDir = (targetPos - worldStartPos).normalized;

        // --- PHASE 1: RECOIL (Ausholen) ---
        float elapsed = 0;
        while (elapsed < recoilDuration)
        {
            transform.position -= attackDir * recoilSpeed * Time.deltaTime;
            elapsed += Time.deltaTime;
            yield return null;
        }

        // --- PHASE 2: Throw (geradliniger Wurf zum Gegner bis zur maximalen Reichweite) ---

        if (weaponStats.weaponProjectileSpeed <= 0f)
        {
            Debug.LogWarning($"{name}: weaponProjectileSpeed is 0 — aborting boomerang throw.");
            transform.SetParent(weaponSocket, true);
            transform.localPosition = Vector3.zero;
            triggerCollider.enabled = false;
            currentState = WeaponState.Idle;
            yield break;
        }

        triggerCollider.enabled = true;

        Vector3 worldRecoilPos = transform.position;

        attackDir = (targetPos - worldRecoilPos).normalized;
        float startAngle = Mathf.Atan2(attackDir.y, attackDir.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0, 0, startAngle + weaponRotationOffset);

        float distanceTraveled = 0f;
        while (distanceTraveled < range)
        {
            float step = weaponStats.weaponProjectileSpeed * Time.deltaTime;
            distanceTraveled += step;
            transform.position += attackDir * step;
            yield return null;
        }
        
        // --- PHASE 3: Wait for return ---
        yield return new WaitForSeconds(postAttackWaitDelay);

        // --- PHASE 4: RETURN (geradliniger Rückflug zum Spieler) ---
        while (Vector3.Distance(transform.position, weaponSocket.position) > weaponStats.weaponProjectileSpeed * Time.deltaTime)
        {
            Vector3 dirToPlayer = (weaponSocket.position - transform.position).normalized;
            transform.position += dirToPlayer * weaponStats.weaponProjectileSpeed * Time.deltaTime;
            yield return null;
        }

        // Abschluss
        transform.SetParent(weaponSocket, true);
        transform.localPosition = Vector3.zero;
        lastAttackTime = Time.time;
        triggerCollider.enabled = false;
        currentState = WeaponState.Idle;
    }

    private void PointWeaponAtEnemy(Transform closestEnemy)
    {
        if (closestEnemy != null)
        {
            Vector3 dir = closestEnemy.position - transform.position;
            float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.Euler(0, 0, angle + weaponRotationOffset);
        }
    }

    private Transform FindClosestEnemy()
    {
        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, enemySearchRadius);
        Transform closestEnemy = null;
        float closestDistance = Mathf.Infinity;

        foreach (var collider in colliders)
        {
            if (collider.CompareTag("Enemy"))
            {
                float distanceToEnemy = Vector2.Distance(transform.position, collider.transform.position);
                if (distanceToEnemy < closestDistance) 
                { 
                    closestDistance = distanceToEnemy; 
                    closestEnemy = collider.transform; 
                }
            }
        }
        return closestEnemy;
    }

    private void ResetWeaponPosition()
    {
        StopAllCoroutines();
        if (transform.parent != weaponSocket)
        {
            transform.SetParent(weaponSocket, true);
        }
        transform.localPosition = Vector3.zero;
        triggerCollider.enabled = false;
        currentState = WeaponState.Idle;
    }
}