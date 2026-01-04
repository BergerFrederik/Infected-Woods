using UnityEngine;

public class ActiveAbilityUI : MonoBehaviour
{
    [SerializeField] private PlayerStats playerStats;
    private float abilityDuration;
    private float startTime;

    private void OnEnable()
    {
        abilityDuration = playerStats.playerAbilityDuration;
        startTime = Time.time;
    }

    // Update is called once per frame
    void Update()
    {
        float passedTime = Time.time - startTime;
        float remainigTime = abilityDuration - passedTime;
        if (remainigTime <= 0)
        {
            this.gameObject.SetActive(false);
        }
    }
}
