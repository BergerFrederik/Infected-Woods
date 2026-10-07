using UnityEngine;

public class PlayerGainsMana : MonoBehaviour
{
    [SerializeField] private PlayerStats playerStats;
    [SerializeField] private float mp_per_second_decimal;
    [SerializeField] private InstantiatePopUp instantiatePopUp;
    private float mpAccumulator;

    private void OnEnable()
    {
        playerStats.OnPlayerInfused += InstantiatePopUp;
    }

    private void OnDisable()
    {
        playerStats.OnPlayerInfused -= InstantiatePopUp;
    }

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
            // Negative regen does nothing - it must not build up a debt that blocks regen later
            mpAccumulator += Mathf.Max(0f, mpPerSecond) * Time.deltaTime;

            if (mpAccumulator >= 1f)
            {
                int wholeMPToGain = Mathf.FloorToInt(mpAccumulator);
                playerStats.playerCurrentMP += wholeMPToGain;
                mpAccumulator -= wholeMPToGain;
            }
        }
    }

    private void InstantiatePopUp(float amount)
    {
        instantiatePopUp.Instantiate(amount, false, transform.root);
    }
}
