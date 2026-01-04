using UnityEngine;

public class PlayerGainsMana : MonoBehaviour
{
    [SerializeField] private PlayerStats playerStats;
    [SerializeField] private float mp_per_second_decimal;
    private float mpAccumulator;

    private void Update()
    {
        GiveManaToPlayerPerSecond();
    }

    private void GiveManaToPlayerPerSecond()
    {
        if (playerStats.playerCurrentMP < playerStats.playerMaxMP)
        {
            // "Stat / 10" = MP per sec
            float mpPerSecond = playerStats.playerMPRegeneration / mp_per_second_decimal;
            mpAccumulator += mpPerSecond * Time.deltaTime;

            if (mpAccumulator >= 1f)
            {
                int wholeMPToGain = Mathf.FloorToInt(mpAccumulator);
                playerStats.playerCurrentMP += wholeMPToGain;
                mpAccumulator -= wholeMPToGain;
            }
        }
    }
}
