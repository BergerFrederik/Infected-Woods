using System.Collections;
using UnityEngine;

public class DamageOverTime : MonoBehaviour
{
    public enum DotType
    {
        Bleed,
        Burn,
        Poison
    }

    [SerializeField] private DotType dotType = DotType.Bleed;
    [HideInInspector] public float dotTime;
    [HideInInspector] public PlayerDealsDamage playerDealsDamage;
    [HideInInspector] public EnemyStats enemyStats;
    [SerializeField] private WeaponStats weaponStats;

    private float _remainingDotTime;

    private void Start()
    {
        _remainingDotTime = dotTime;
        StartCoroutine(DotCoroutine());
    }

    private void OnDestroy()
    {
        StopAllCoroutines();
    }

    private IEnumerator DotCoroutine()
    {
        float tickTimer = 0f;

        while (_remainingDotTime > 0)
        {
            float dt = Time.deltaTime;
            _remainingDotTime -= dt;
            tickTimer += dt;

            if (tickTimer >= 1f)
            {
                tickTimer -= 1f;
                playerDealsDamage.ApplyNonCritableDamageToEnemy(enemyStats, weaponStats);
            }

            yield return null;
        }
        
        Destroy(gameObject);
    }
}
