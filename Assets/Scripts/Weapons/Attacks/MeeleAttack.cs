using System.Collections;
using UnityEngine;


public class MeeleAttack : MonoBehaviour
{
    [Header("Weapon Settings")]
    [SerializeField] private WeaponStats weaponStats;
    [SerializeField] private float weaponRotationOffset = 0f;
    
    [Header("Attack Timings")]
    [SerializeField] private float recoilDuration = 0.1f;
    [SerializeField] private float thrustDuration = 0.05f;
    [SerializeField] private float returnDuration = 0.2f;
    [SerializeField] private float postAttackWaitDelay = 0.15f;

    [Header("Attack Distances")]
    [SerializeField] private float recoilDistance = 0.5f;
    [SerializeField] private float enemySearchRadius = 20f;

    
    private enum WeaponState { Idle, Attacking }

    private WeaponState currentState = WeaponState.Idle;
    
    private PlayerStats playerStats;
    private BoxCollider2D triggerCollider;

    private float lastAttackTime;

    private void Awake()
    {
        triggerCollider = this.gameObject.GetComponent<BoxCollider2D>();
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
        
        float currentRange = weaponStats.weaponRange * (1f + playerStats.playerAttackRange / 100f);
        
        if (Vector2.Distance(transform.position, closestEnemy.position) <= currentRange)
        {
            StartCoroutine(ThrustAttackRoutine(closestEnemy.position, currentRange));
        }
    }
    
    
    private IEnumerator ThrustAttackRoutine(Vector3 targetPos, float range)
    {
        currentState = WeaponState.Attacking;
        
        // Richtung beim Start des Angriffs fixieren
        Vector3 attackDir = (targetPos - transform.position).normalized;
        Vector3 startLocalPos = Vector3.zero;

        // --- PHASE 1: RECOIL (Ausholen) ---
        float elapsed = 0;
        Vector3 recoilPos = startLocalPos - attackDir * recoilDistance;
        while (elapsed < recoilDuration)
        {
            transform.localPosition = Vector3.Lerp(startLocalPos, recoilPos, elapsed / recoilDuration);
            elapsed += Time.deltaTime;
            yield return null;
        }

        // --- PHASE 2: THRUST (Zustoßen) ---

        triggerCollider.enabled = true; 
        elapsed = 0;
        Vector3 targetLocalPos = startLocalPos + attackDir * range;
        while (elapsed < thrustDuration)
        {
            transform.localPosition = Vector3.Lerp(recoilPos, targetLocalPos, elapsed / thrustDuration);
            elapsed += Time.deltaTime;
            yield return null;
        }
        transform.localPosition = targetLocalPos;

        // --- PHASE 3: WAIT (Kurzes Verweilen am Zielpunkt) ---
        yield return new WaitForSeconds(postAttackWaitDelay);

        // --- PHASE 4: RETURN (Zurückkehren) ---
        triggerCollider.enabled = false; 
        elapsed = 0;
        while (elapsed < returnDuration)
        {
            transform.localPosition = Vector3.Lerp(targetLocalPos, startLocalPos, elapsed / returnDuration);
            elapsed += Time.deltaTime;
            yield return null;
        }

        // Abschluss
        transform.localPosition = startLocalPos;
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