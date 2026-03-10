using UnityEngine;

[CreateAssetMenu(fileName = "NewUnit", menuName = "WaveSystem/Unit")]
public class WaveDataUnit : ScriptableObject
{
    public GameObject enemyUnit;
    public float minNumber;
    public float maxNumber;
    public float spawnChance;
    public float additionalMinDistanceFromPlayer;
}
