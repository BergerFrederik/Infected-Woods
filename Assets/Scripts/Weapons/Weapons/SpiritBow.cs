using System.Collections;
using UnityEngine;

public class SpiritBow : MonoBehaviour
{
    [SerializeField] private float chanceToGainAttackspeed;
    [SerializeField] private float attackspeedBoost;
    [SerializeField] private float attackspeedBoostTime;
    [SerializeField] private Ranged ranged;
    
    private GameObject _playerObject;
    private PlayerStats _playerStats;
    private RandomRollEvent _randomRollEvent;

    private void Start()
    {
        _playerObject = GameObject.FindGameObjectWithTag("Player");
        _playerStats = _playerObject.GetComponent<PlayerStats>();
        _randomRollEvent = _playerObject.GetComponentInChildren<RandomRollEvent>();
        ranged.OnWeaponProjectileHitsEnemy += IncreaseAttackSpeed;
    }

    private void OnDestroy()
    {
        ranged.OnWeaponProjectileHitsEnemy -= IncreaseAttackSpeed;
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
        _playerStats.playerAttackSpeed += attackspeedBoost;
        yield return new WaitForSeconds(attackspeedBoostTime);
        _playerStats.playerAttackSpeed -= attackspeedBoost;
    }
}
