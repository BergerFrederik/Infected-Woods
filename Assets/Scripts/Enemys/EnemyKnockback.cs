using UnityEngine;

public class EnemyKnockback : MonoBehaviour
{
    [SerializeField] private EnemyStats enemyStats;
    [SerializeField] private float knockbackDuration = 0.2f;
    [SerializeField] private AnimationCurve knockbackCurve = AnimationCurve.Linear(0, 0, 1, 1);

    private Transform _playerTransform;
    private PlayerStats _playerStats;
    
    private bool isKnockedBack;
    private float knockbackStrength;
    private float startTime;

    public bool useLerpResistance { get; set; } = true;
    public bool canReceiveKnockback { get; set; } = true;

    private void Awake()
    {
        var player = GameObject.FindGameObjectWithTag("Player");
        _playerTransform = player.transform;
        _playerStats = player.GetComponent<PlayerStats>();
    }

    public void ApplyMovement(Vector3 moveVector)
    {
        if (!isKnockedBack)
        {
            transform.position += moveVector * Time.deltaTime;
            return;
        }

        float factor = Mathf.Clamp01((Time.time - startTime) / knockbackDuration);
        
        if (factor >= 1f)
        {
            isKnockedBack = false;
            transform.position += moveVector * Time.deltaTime;
            return;
        }

        Vector3 knockbackDir = (transform.position - _playerTransform.position).normalized;
        Vector3 knockbackVector = knockbackDir * knockbackStrength;

        if (useLerpResistance)
        {
            float lerpFactor = knockbackCurve.Evaluate(factor);
            Vector3 actualMove = Vector3.Lerp(knockbackVector, moveVector, lerpFactor);
            transform.position += actualMove * Time.deltaTime;
        }
        else
        {
            // Direkter Knockback ohne Gegensteuern
            transform.position += knockbackVector * Time.deltaTime;
        }
    }

    private void OnTriggerEnter2D(Collider2D collider)
    {
        if (!canReceiveKnockback) return;

        if (collider.TryGetComponent<WeaponStats>(out WeaponStats weaponStats))
        {
            if (!isKnockedBack)
            {
                startTime = Time.time;
                float pKnock = _playerStats.playerKnockback / 100f;
                float wKnock = weaponStats.weaponKnockback;
                float res = enemyStats.enemyKnockbackResistance;

                knockbackStrength = (wKnock + wKnock * pKnock) * (1 - (res / 100f));
                if (knockbackStrength > 0) isKnockedBack = true;
            }
        }
    }
}
