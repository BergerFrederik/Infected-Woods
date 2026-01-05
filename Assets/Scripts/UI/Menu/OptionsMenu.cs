using UnityEngine;
using UnityEngine.UI;

public class OptionsMenu : MonoBehaviour
{
    [SerializeField] private GameObject PlayerStatsUI;
    [SerializeField] private Toggle enablePlayerStatsUIToggle;

    private void Start()
    {
        enablePlayerStatsUIToggle.onValueChanged.AddListener(delegate
        {
            ToggleStats(enablePlayerStatsUIToggle.isOn);
        });
    }

    private void ToggleStats(bool isEnabled)
    {
        PlayerStatsUI.SetActive(isEnabled);
    }

}
