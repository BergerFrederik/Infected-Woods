using System.Collections;
using UnityEngine;

public class SpiritBow : MonoBehaviour
{
    [SerializeField] private float _chanceToGainAttackspeed;
    [SerializeField] private float _attackspeedBoost;
    [SerializeField] private float _attackspeedBoostTime;
    [SerializeField] private Ranged _ranged;
    
    private GameObject _playerObject;
    private PlayerStats _playerStats;
    private RandomRollEvent _randomRollEvent;

    private void Start()
    {
        _playerObject = GameObject.FindGameObjectWithTag("Player");
        _playerStats = _playerObject.GetComponent<PlayerStats>();
        _randomRollEvent = _playerObject.GetComponentInChildren<RandomRollEvent>();
       _ranged.OnWeaponProjectileHitsEnemy += IncreaseAttackSpeed;
    }

    private void OnDestroy()
    {
        _ranged.OnWeaponProjectileHitsEnemy -= IncreaseAttackSpeed;
    }

    private void IncreaseAttackSpeed()
    {
        float rndNum = _randomRollEvent.GetRandomFloatRoll(0f, 100f);
        if (rndNum < 1 - _chanceToGainAttackspeed) //Muss 1- sein, damit luck einen Einfluss hat. Luck erhöht den Roll
        {
            StartCoroutine(GainAttackSpeedForSeconds());
        }
    }

    private IEnumerator GainAttackSpeedForSeconds()
    {
        _playerStats.playerAttackSpeed += _attackspeedBoost;
        yield return new WaitForSeconds(_attackspeedBoostTime);
        _playerStats.playerAttackSpeed -= _attackspeedBoost;
    }
}
