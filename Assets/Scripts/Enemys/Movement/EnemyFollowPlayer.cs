using UnityEngine;



public class EnemyFollowPlayer : MonoBehaviour
{
    [SerializeField] private EnemyStats enemyStats;
    [SerializeField] private Pathfinder pathfinder;
    [SerializeField] private float knockback_duration = .2f;
    [SerializeField] private AnimationCurve knockbackCurve = AnimationCurve.Linear(0, 0, 1, 1);

    private Transform _playerTransform;
    private PlayerStats _playerStats;
    
    private bool isKnockedBack;
    
    float playerKnockback;
    float weaponKnockback;
    float knockbackResistance;
    float knockbackStrength;
    float startTime;


    private void Start()
    {
        _playerTransform = GameObject.FindGameObjectWithTag("Player").transform;
        _playerStats = _playerTransform.GetComponent<PlayerStats>();
    }
    private void Update()
    {
        Vector2 moveDir = pathfinder.CalculateEnemyMovementVector();

        var moveVector = moveDir * enemyStats.enemyMoveSpeed;
        var knockbackVector = (transform.position - _playerTransform.position).normalized * knockbackStrength;
        
        if (isKnockedBack)
        {
            if (Time.time - startTime > knockback_duration)
            {
                isKnockedBack = false;
            }
        }

        var factor = Mathf.Clamp01((Time.time - startTime) /  knockback_duration);
        var lerpFactor = knockbackCurve.Evaluate(factor);
        var actualMoveVector = Vector3.Lerp(knockbackVector, moveVector, lerpFactor);
        transform.position += (Vector3)actualMoveVector * Time.deltaTime;
    }
    
    private void OnTriggerEnter2D(Collider2D collider)
    {
        if (collider.TryGetComponent<WeaponStats>(out WeaponStats weaponStats))
        {
            if (!isKnockedBack)
            {
                startTime = Time.time;
                playerKnockback = _playerStats.playerKnockback / 100;
                weaponKnockback = weaponStats.weaponKnockback;
                knockbackResistance = enemyStats.enemyKnockbackResistance;
                knockbackStrength = (weaponKnockback + weaponKnockback * playerKnockback) * (1 - (knockbackResistance / 100));
                if (knockbackStrength > 0)
                {
                    isKnockedBack = true;
                }
            }
        }
    }
}


    