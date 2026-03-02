using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class WaveManager : MonoBehaviour
{
    [HideInInspector] public WaveData currentWave;
    [SerializeField] private EntitySpawner entitySpawner; 

    private bool _isWaveActive;
    private List<Coroutine> _activeCoroutines = new List<Coroutine>();

    public void StartWave()
    {
        _isWaveActive = true;
        foreach (var group in currentWave.groupsData)
        {
            _activeCoroutines.Add(StartCoroutine(SpawnGroupRoutine(group)));
        }
    }

    public void StopWave()
    {
        _isWaveActive = false;
        foreach (var c in _activeCoroutines)
        {
            if (c != null) StopCoroutine(c);
        }
        _activeCoroutines.Clear();
    }

    private IEnumerator SpawnGroupRoutine(WaveGroupData group)
    {
        yield return new WaitForSeconds(group.spawnTiming);

        int spawnsPerformed = 0;
        while (spawnsPerformed <= group.repeating && _isWaveActive)
        {
            entitySpawner.AddGroupToQueue(group);
            
            spawnsPerformed++;
            yield return new WaitForSeconds(group.repeatingInterval);
        }
    }
}