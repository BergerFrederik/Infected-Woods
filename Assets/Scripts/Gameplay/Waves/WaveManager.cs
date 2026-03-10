using UnityEngine;
using System.Collections.Generic;

public class WaveManager : MonoBehaviour
{
    [System.Serializable] // Macht die Klasse im Debugger/Inspector sichtbar (optional)
    public class RepeatingGroup
    {
        public WaveGroupData data;
        public int remainingRepeats;
        public float currentInterval;
        public float nextSpawnTime;

        // Der Konstruktor: Wird aufgerufen, wenn wir "new RepeatingGroup(...)" schreiben
        public RepeatingGroup(WaveGroupData groupData, float startTime)
        {
            data = groupData;
            remainingRepeats = groupData.repeating;
            currentInterval = groupData.repeatingInterval;
            nextSpawnTime = startTime + groupData.repeatingInterval;
        }
    }
    
    
    [Header("References")]
    [SerializeField] private EntitySpawner entitySpawner;
    public WaveData currentWave;

    [Header("State")]
    private float _waveTimer = 0f;
    private bool _isWaveActive = false;
    private int _lastTimeChecked = -1;

    // Listen für die Verwaltung (Entspricht _groups_to_repeat etc. in Godot)
    private List<WaveGroupData> _pendingGroups = new List<WaveGroupData>();
    private List<RepeatingGroup> _activeRepeatingGroups = new List<RepeatingGroup>();

    public void StartWave(WaveData wave)
    {
        currentWave = wave;
        _waveTimer = 0f;
        _isWaveActive = true;
        _lastTimeChecked = -1;

        // Wir kopieren die Gruppen in eine Arbeitsliste, damit wir das Scriptable Object nicht zerstören
        _pendingGroups = new List<WaveGroupData>(currentWave.groupsData);
        _activeRepeatingGroups.Clear();
    }

    private void Update()
    {
        if (!_isWaveActive || currentWave == null) return;

        _waveTimer += Time.deltaTime;
        int secondsElapsed = Mathf.FloorToInt(_waveTimer);

        // Nur einmal pro Sekunde die Hauptprüfung machen (wie im Godot Code)
        if (secondsElapsed != _lastTimeChecked)
        {
            _lastTimeChecked = secondsElapsed;
            CheckForNewSpawns(secondsElapsed);
        }

        // Wiederholende Gruppen prüfen
        CheckRepeatingGroups(_waveTimer);

        // Wellenende prüfen
        if (_waveTimer >= currentWave.waveDuration)
        {
            EndWave();
        }
    }

    private void CheckForNewSpawns(float timeElapsed)
    {
        for (int i = _pendingGroups.Count - 1; i >= 0; i--)
        {
            WaveGroupData group = _pendingGroups[i];

            // Prüfung: Schwierigkeit und Timing (Brotato Logik)
            // Hinweis: RunData.current_difficulty müsstest du ggf. noch durch dein System ersetzen
            if (timeElapsed >= group.spawnTiming)
            {
                // Chance prüfen
                if (Random.Range(0f, 1f) <= group.spawnChance)
                {
                    SpawnGroup(group);
                }

                // Wenn sie sich wiederholt, in die Repeat-Liste packen
                if (group.repeating > 0)
                {
                    _activeRepeatingGroups.Add(new RepeatingGroup(group, timeElapsed));
                }

                // Aus der Pending-Liste entfernen (entspricht _groups_to_remove)
                _pendingGroups.RemoveAt(i);
            }
        }
    }

    private void CheckRepeatingGroups(float currentTime)
    {
        for (int i = _activeRepeatingGroups.Count - 1; i >= 0; i--)
        {
            RepeatingGroup rep = _activeRepeatingGroups[i];

            if (currentTime >= rep.nextSpawnTime)
            {
                SpawnGroup(rep.data);
                rep.remainingRepeats--;

                if (rep.remainingRepeats <= 0)
                {
                    _activeRepeatingGroups.RemoveAt(i);
                }
                else
                {
                    // Intervall reduzieren (Brotato Logik)
                    rep.currentInterval = Mathf.Max(rep.data.minRepeatingInterval, 
                                          rep.currentInterval - rep.data.reduceRepeatingInterval);
                    rep.nextSpawnTime = currentTime + rep.currentInterval;
                }
            }
        }
    }

    private void SpawnGroup(WaveGroupData group)
    {
        entitySpawner.AddGroupToQueue(group);
    }

    public void EndWave()
    {
        _isWaveActive = false;
        entitySpawner.ClearEnemies();
    }
}