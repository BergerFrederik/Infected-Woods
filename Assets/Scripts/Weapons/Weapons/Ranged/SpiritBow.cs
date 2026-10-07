using System.Collections;
using UnityEngine;

public class SpiritBow : MonoBehaviour
{
    [SerializeField] private float chanceToGainAttackspeed;
    [SerializeField] private float chanceIncrease;
    [SerializeField] private float attackspeedBoost;
    [SerializeField] private float attackspeedIncrease;
    [SerializeField] private float attackspeedBoostTime;
    [SerializeField] private float timeIncrease;
    [SerializeField] private Ranged ranged;
    [SerializeField] private WeaponStats weaponStats;
    
    private GameObject _playerObject;
    private PlayerStats _playerStats;
    private RandomRollEvent _randomRollEvent;
    private float _activeAttackspeedBoost;

    private void Start()
    {
        _playerObject = GameObject.FindGameObjectWithTag("Player");
        _playerStats = _playerObject.GetComponent<PlayerStats>();
        _randomRollEvent = _playerObject.GetComponentInChildren<RandomRollEvent>();
        ranged.OnWeaponProjectileHitsEnemy += IncreaseAttackSpeed;
        weaponStats.OnWeaponLevelChanged += UpdateStats;
    }

    private void OnDestroy()
    {
        ranged.OnWeaponProjectileHitsEnemy -= IncreaseAttackSpeed;
        weaponStats.OnWeaponLevelChanged -= UpdateStats;
    }

    // Coroutines stop when the bow is sold, merged or put on the bench - take back the boosts
    // they would have removed, otherwise the attack speed stays forever.
    private void OnDisable()
    {
        StopAllCoroutines();
        if (_playerStats != null)
        {
            _playerStats.playerAttackSpeed -= _activeAttackspeedBoost;
        }
        _activeAttackspeedBoost = 0f;
    }

    private void IncreaseAttackSpeed()
    {
        float rndNum = _randomRollEvent.GetRandomFloatRoll(0f, 100f);
        if (rndNum > 100f - chanceToGainAttackspeed) //Muss 100- sein, damit luck einen Einfluss hat. Luck erhöht den Roll
        {
            StartCoroutine(GainAttackSpeedForSeconds());
        }
    }

    private IEnumerator GainAttackSpeedForSeconds()
    {
        // Remember the value - a level up during the boost changes attackspeedBoost
        float boost = attackspeedBoost;
        _playerStats.playerAttackSpeed += boost;
        _activeAttackspeedBoost += boost;
        yield return new WaitForSeconds(attackspeedBoostTime);
        _playerStats.playerAttackSpeed -= boost;
        _activeAttackspeedBoost -= boost;
    }

    private void UpdateStats(float weaponLevel)
    {
        chanceToGainAttackspeed = RoundUpToHalf(chanceToGainAttackspeed * chanceIncrease);
        attackspeedBoost = RoundUpToHalf(attackspeedBoost * attackspeedIncrease);
        attackspeedBoostTime = RoundUpToHalf(attackspeedBoostTime * timeIncrease);
    }

    private static float RoundUpToHalf(float value) => Mathf.Ceil(value * 2f) / 2f;
}
