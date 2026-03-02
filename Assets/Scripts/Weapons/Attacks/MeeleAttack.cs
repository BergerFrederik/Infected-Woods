using System.Collections;
using UnityEngine;


public class MeeleAttack : MonoBehaviour
{
    [Header("Weapon Settings")]
    [SerializeField] private WeaponStats weaponStats;
    [SerializeField] private float weaponRotationOffset = 0f;
    
    [Header("Attack Timings")]
    [SerializeField] private float recoilDuration = 0.1f;
    [SerializeField] private float attackSpeedUnit = 10f; 
    [SerializeField] private float returnSpeedUnit = 5f;
    [SerializeField] private float postAttackWaitDelay = 0.15f;

    [Header("Attack Distances")]
    [SerializeField] private float recoilDistance = 0.5f;
    [SerializeField] private float enemySearchRadius = 20f;

    
    private enum WeaponState { Idle, Attacking }

    private WeaponState currentState = WeaponState.Idle;
    
    private PlayerStats playerStats;
    private Collider2D triggerCollider;

    private float lastAttackTime;

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
        if (distanceToEdge <= attackRange)
        {
            StartCoroutine(ThrustAttackRoutine(closestEnemy.position, attackRange));
        }
    }
    
    
    private IEnumerator ThrustAttackRoutine(Vector3 targetPos, float range)
    {
        currentState = WeaponState.Attacking;
        
        // Richtung beim Start des Angriffs fixieren
        
        Vector3 worldStartPos = transform.position;
        Vector3 attackDir = (targetPos - worldStartPos).normalized;
        
        Vector3 worldRecoilPos = worldStartPos - attackDir * recoilDistance;
        Vector3 worldTargetPos = worldStartPos + attackDir * range;

        // --- PHASE 1: RECOIL (Ausholen) ---
        float elapsed = 0;
        while (elapsed < recoilDuration)
        {
            transform.position = Vector3.Lerp(worldStartPos, worldRecoilPos, elapsed / recoilDuration);
            elapsed += Time.deltaTime;
            yield return null;
        }
        transform.position = worldRecoilPos;
        
        // --- PHASE 2: THRUST (Zustoßen) ---

        triggerCollider.enabled = true; 
        elapsed = 0;
        
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