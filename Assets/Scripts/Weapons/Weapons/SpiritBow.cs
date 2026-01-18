using System;
using System.Collections;
using UnityEngine;
using Random = UnityEngine.Random;

public class SpiritBow : MonoBehaviour
{
    [SerializeField] private float _chanceToGainAttackspeed;
    [SerializeField] private float _attackspeedBoost;
    [SerializeField] private float _attackspeedBoostTime;
    [SerializeField] private Ranged _ranged;
    private GameObject _playerObject;
    private PlayerStats _playerStats;

    private void Start()
    {
        _playerObject = GameObject.FindGameObjectWithTag("Player");
        _playerStats = _playerObject.GetComponent<PlayerStats>();
       _ranged.OnWeaponProjectileHitsEnemy += IncreaseAttackSpeed;
    }

    private void OnDestroy()
    {
        _ranged.OnWeaponProjectileHitsEnemy -= IncreaseAttackSpeed;
    }

    private void IncreaseAttackSpeed()
    {
        int rndNum = Random.Range(0, 100);
        if (rndNum < _chanceToGainAttackspeed)
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
