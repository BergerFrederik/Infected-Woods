using System.Collections;
using UnityEngine;


public class Boomerang : MonoBehaviour
{
    [Header("Weapon Settings")]
    [SerializeField] private WeaponStats weaponStats;
    [SerializeField] private float weaponRotationOffset = 0f;
    [SerializeField] private float numFlights = 1f;

    [Header("Visuals")]
    [SerializeField] private float spinSpeed = 720f; // degrees per second

    [Header("Attack Timings")]
    [SerializeField] private float recoilDuration = 0.1f;
    [SerializeField] private float recoilSpeed = 4f;
    [SerializeField] private float postAttackWaitDelay = 0.1f;

    [Header("Attack Distances")]
    [SerializeField] private float enemySearchRadius = 20f;

    [Header("Flight Smoothing")]
    [SerializeField] private float overshootDistance = 0.5f;
    [SerializeField] private float overshootDuration = 0.15f;


    private enum WeaponState { Idle, Attacking }

    private WeaponState currentState = WeaponState.Idle;
    
    private PlayerStats playerStats;
    private Collider2D triggerCollider;

    private float lastAttackTime;
    private float weaponLengthOffset;
    private float _attackRange;
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
                if (CheckForAttack(closestEnemy))
                {
                    StartCoroutine(BoomerangFlyRoutine(closestEnemy));
                }
            }
        }
    }
    
    private bool CheckForAttack(Transform closestEnemy)
    {
        if (closestEnemy == null) return false;
        Collider2D enemyCollider = closestEnemy.GetComponent<Collider2D>();
        Vector2 closestPointOnEdge = enemyCollider.ClosestPoint(transform.position);
        float distanceToEdge = Vector2.Distance(transform.position, closestPointOnEdge);
        _attackRange = weaponStats.weaponRange + weaponStats.weaponRange * (playerStats.playerAttackRange / 100f);
        
        if (distanceToEdge <= weaponLengthOffset + _attackRange)
        {
            return true;
        }
        return false;
    }
    
    
    private IEnumerator BoomerangFlyRoutine(Transform targetEnemy)
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
        for (int i = 1; i <= numFlights; i++)
        {
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
            targetPos = targetEnemy.position;

            attackDir = (targetPos - worldRecoilPos).normalized;
            float startAngle = Mathf.Atan2(attackDir.y, attackDir.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.Euler(0, 0, startAngle + weaponRotationOffset);

            float distanceTraveled = 0f;
            while (distanceTraveled < _attackRange)
            {
                float step = weaponStats.weaponProjectileSpeed * Time.deltaTime;
                distanceTraveled += step;
                transform.position += attackDir * step;
                transform.Rotate(0f, 0f, spinSpeed * Time.deltaTime);
                yield return null;
            }

            // Ease out into a short overshoot instead of stopping instantly
            Vector3 overshootStart = transform.position;
            float decelElapsed = 0f;
            while (decelElapsed < overshootDuration)
            {
                float t = decelElapsed / overshootDuration;
                float easedT = 1f - (1f - t) * (1f - t); // ease-out quadratic
                transform.position = overshootStart + attackDir * overshootDistance * easedT;
                transform.Rotate(0f, 0f, spinSpeed * Time.deltaTime);
                decelElapsed += Time.deltaTime;
                yield return null;
            }
            transform.position = overshootStart + attackDir * overshootDistance;

            // --- PHASE 3: Wait for return ---
            float waitElapsed = 0f;
            while (waitElapsed < postAttackWaitDelay)
            {
                transform.Rotate(0f, 0f, spinSpeed * Time.deltaTime);
                waitElapsed += Time.deltaTime;
                yield return null;
            }

            if (i < numFlights)
            {
                Transform nextTarget = FindClosestEnemy();
                if (!CheckForAttack(nextTarget))
                {
                    break;
                }
                targetEnemy = nextTarget;
            }
        }
        
        // --- PHASE 4: RETURN (geradliniger Rückflug zum Spieler) ---
        while (Vector3.Distance(transform.position, weaponSocket.position) > weaponStats.weaponProjectileSpeed * Time.deltaTime)
        {
            Vector3 dirToPlayer = (weaponSocket.position - transform.position).normalized;
            transform.position += dirToPlayer * weaponStats.weaponProjectileSpeed * Time.deltaTime;
            transform.Rotate(0f, 0f, spinSpeed * Time.deltaTime);
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