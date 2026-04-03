using UnityEngine;

public class RandomRollEvent : MonoBehaviour
{
    [SerializeField] private PlayerStats playerStats;

    public float GetRandomFloatRoll(float minInclusive, float maxInclusive)
    {
        float roll = Random.Range(minInclusive, maxInclusive);
        float luck = playerStats.playerLuck;
        return roll + luck;
    }
    
    public int GetRandomIntRoll(int minInclusive, int maxExclusive)
    {
        int roll = Random.Range(minInclusive, maxExclusive);
        int luck = (int)playerStats.playerLuck;
        return roll + luck;
    }
}
