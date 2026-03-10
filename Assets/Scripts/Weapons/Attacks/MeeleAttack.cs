using System.Collections;
using UnityEngine;


public class MeeleAttack : MonoBehaviour
{
    [Header("Weapon Settings")]
    [SerializeField] private WeaponStats weaponStats;
    [SerializeField] private float weaponRotationOffset = 0f;
    
    [Header("Attack Timings")]
    [SerializeField] private float recoilDuration = 0.1f;
    [SerializeField] private float recoilSpeed = 4f;
    [SerializeField] private float attackSpeedUnit = 10f; 
    [SerializeField] private float returnSpeedUnit = 5f;
    [SerializeField] private float postAttackWaitDelay = 0.15f;

    [Header("Attack Distances")]
    [SerializeField] private float enemySearchRadius = 20f;

    
    private enum WeaponState { Idle, Attacking }

    private WeaponState currentState = WeaponState.Idle;
    
    private PlayerStats playerStats;
    private Collider2D triggerCollider;

    private float lastAttackTime;
    private float weaponLengthOffset;

    private void Awake()
    {
        triggerCollider = this.gameObject.GetComponent<Collider2D>();
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
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            playerStats = player.GetComponent<PlayerStats>();
        }
        
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

        weaponLengthOffset = 1.1f; // correction factor to ensure correct calculation
    }

    private void OnDestroy()
    {
        GameManager.OnRoundOver -= ResetWeaponPosition;
    }

    private void Update()
    {
        if (currentState == WeaponState.Idle)
        {
            Transform closestEnemy = FindClosestEnemy();
            PointWeaponAtEnemy(closestEnemy);
            
            float attackCooldown = weaponStats.weaponAttackSpeedCooldown / (1f + playerStats.playerAttackSpeed / 100f);
            
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
            StartCoroutine(ThrustAttackRoutine(closestEnemy, attackRange));
        }
    }
    
    
    private IEnumerator ThrustAttackRoutine(Transform targetEnemy, float range)
    {
        Vector3 targetPos = targetEnemy.position;
        currentState = WeaponState.Attacking;
        
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
        
        // --- PHASE 2: THRUST (Zustoßen) ---

        triggerCollider.enabled = true; 
        elapsed = 0;
        
        Vector3 worldRecoilPos = transform.position;
        
        attackDir = (targetPos - worldRecoilPos).normalized;
        Vector3 worldTargetPos = worldRecoilPos + attackDir * range;

        float angle = Mathf.Atan2(attackDir.y, attackDir.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0, 0, angle + weaponRotationOffset);
        
        float thrustDuration = Vector3.Distance(worldTargetPos, worldRecoilPos) / attackSpeedUnit;
        while (elapsed < thrustDuration)
        {
            transform.position = Vector3.Lerp(worldRecoilPos, worldTargetPos, elapsed / thrustDuration);
            elapsed += Time.deltaTime;
            yield return null;
        }
        transform.position = worldTargetPos;

        // --- PHASE 3: WAIT (Kurzes Verweilen am Zielpunkt) ---
        yield return new WaitForSeconds(postAttackWaitDelay);

        // --- PHASE 4: RETURN (Zurückkehren) ---
        triggerCollider.enabled = false; 
        elapsed = 0;
        
        Vector3 positionAtReturnStart = transform.position;
        float returnDuration = Vector3.Distance(positionAtReturnStart, transform.parent.position) / returnSpeedUnit;
        
        while (elapsed < returnDuration)
        {
            transform.position = Vector3.Lerp(positionAtReturnStart, transform.parent.position, elapsed / returnDuration);
            elapsed += Time.deltaTime;
            yield return null;
        }

        // Abschluss
        transform.localPosition = Vector3.zero;
        lastAttackTime = Time.time;
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
        transform.localPosition = Vector3.zero;
        triggerCollider.enabled = false;
        currentState = WeaponState.Idle;
    }
}