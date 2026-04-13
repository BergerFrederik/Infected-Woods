using UnityEngine;

public class RandomRollEvent : MonoBehaviour
{
    [SerializeField] private PlayerStats playerStats;
    [SerializeField] private float maximumFlatLuckIncrease = 50f;
    [SerializeField] private float maximumFlatLuckDecrease = -50f;

    public float GetRandomFloatRoll(float minInclusive, float maxInclusive)
    {
        float roll = Random.Range(minInclusive, maxInclusive);
        float luck = Mathf.Clamp(playerStats.playerLuck/10f, maximumFlatLuckDecrease, maximumFlatLuckIncrease); // 10f to make the number ingame bigger
        float finalValue = Mathf.Clamp(roll + luck, 0f,  100f);
        return finalValue;
    }
    
    public int GetRandomIntRoll(int minInclusive, int maxExclusive)
    {
        int roll = Random.Range(minInclusive, maxExclusive);
        int luck = (int)Mathf.Clamp(playerStats.playerLuck/10f, maximumFlatLuckDecrease, maximumFlatLuckIncrease); // 10f to make the number ingame bigger
        int finalValue = Mathf.Clamp(roll + luck, 0,  100);
        return finalValue;
    }
}
