using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SetStats : MonoBehaviour
{
    [SerializeField] private TMP_Dropdown statDropdown;
    [SerializeField] private TMP_InputField statInputField;
    [SerializeField] private Button setButton;
    [SerializeField] private PlayerStats playerStats;

    private void OnEnable()
    {
        setButton.onClick.AddListener(SetInputToStats);
    }

    private void SetInputToStats()
    {
        float input = float.Parse(statInputField.text);
        string statName = statDropdown.options[statDropdown.value].text;
        switch (statName)
        {
            case "HP":
                playerStats.playerMaxHP = input;
                break;
            case "HP Reg":
                playerStats.playerHPRegeneration = input;
                break;
            case "Lifesteal":
                playerStats.playerLifeSteal = input;
                break;
            case "Damage":
                playerStats.playerDamage = input;
                break;
            case "AS":
                playerStats.playerAttackSpeed = input;
                break;
            case "Crit Chance":
                playerStats.playerCritChance = input;
                break;
            case "MS":
                playerStats.playerMovespeed = input;
                break;
            case "MP":
                playerStats.playerMaxMP = input;
                break;
            case "MP Reg":
                playerStats.playerMPRegeneration = input;
                break;
        }
    }
}
