using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CharacterSelection : MonoBehaviour
{
    [SerializeField] private Button returnButton;
    [SerializeField] private Button selectCharacterButton;
    [SerializeField] private GameObject DifficultySelectionPanel;
    [SerializeField] private GameObject TitleScreen;
    [SerializeField] private GameObject PlayerCharacterList;
    [SerializeField] private GameObject[] PlayerCharacters;
    [SerializeField] private GameObject[] WeaponList;
    [SerializeField] private GameObject CharacterButtonPrefab;
    
    [Header("UI)")]
    [SerializeField] private TextMeshProUGUI characterNameText;
    [SerializeField] private TextMeshProUGUI characterPassiveText;
    [SerializeField] private TextMeshProUGUI characterActiveText;
    [SerializeField] private TextMeshProUGUI characterActiveDescriptionText;
    [SerializeField] private TextMeshProUGUI weaponNameText;
    [SerializeField] private TextMeshProUGUI weaponStatsText;
    [SerializeField] private TextMeshProUGUI weaponSpecialsText;
    
    private Dictionary<string, GameObject> _availableWeaponsMap = new Dictionary<string, GameObject>(); 

    private void OnEnable()
    {
        returnButton.onClick.AddListener(() => ReturnToMenu());
        selectCharacterButton.onClick.AddListener(() => SelectCharacter());
    }
    
    private void OnDisable()
    {
        returnButton.onClick.RemoveAllListeners();
        selectCharacterButton.onClick.RemoveAllListeners();
    }

    private void Start()
    {
        for (int i = 0; i < WeaponList.Length; i++)
        {
            string weaponName = WeaponList[i].name;
            _availableWeaponsMap.Add(weaponName, WeaponList[i]);
        }
        LoadCharacters();
    }

    private void LoadCharacters()
    {
        foreach (GameObject character in PlayerCharacters)
        {
            GameObject newCharacterButtonObject = Instantiate(CharacterButtonPrefab,  PlayerCharacterList.transform);
            newCharacterButtonObject.transform.GetChild(0).GetComponent<Image>().sprite = character.GetComponentInChildren<SpriteRenderer>().sprite;
            OnHover onHover = newCharacterButtonObject.GetComponentInChildren<OnHover>();
            onHover.OnCursorHoverEnter += DisplayCharacterInformation;
            onHover.OnCursorHoverExit += ClearCharacterInformation;
            SetInformationToButton(character, newCharacterButtonObject);
        }
    }

    private void SetInformationToButton(GameObject character, GameObject CharacterButton)
    {
        CharacterSelectionButtonInformation characterInformation = CharacterButton.GetComponentInChildren<CharacterSelectionButtonInformation>();
        CharacterStats characterStats = character.GetComponentInChildren<CharacterStats>();
        GameObject characterWeapon = _availableWeaponsMap[characterStats.startWeaponID];
        WeaponStats weaponStats = characterWeapon.GetComponent<WeaponStats>();
        
        characterInformation.CharacterName = character.name;
        characterInformation.CharacterPassive = "";
        characterInformation.CharacterActiveCooldown = characterStats.ability_cooldown.ToString();
        characterInformation.CharacterActiveDuration = characterStats.ability_duration.ToString();
        characterInformation.CharacterActiveManaCost = characterStats.ability_manaCost.ToString();
        characterInformation.CharacterActiveDescription = characterStats.abilityDescription;
        characterInformation.WeaponName = characterWeapon.name;
        characterInformation.WeaponDamage = weaponStats.weaponBaseDamage.ToString();
        characterInformation.WeaponAttackSpeed = weaponStats.weaponAttackSpeedCooldown.ToString();
        characterInformation.WeaponSpecialAbility= weaponStats.weaponSpecialAbility;
    }
    
    private void DisplayCharacterInformation(GameObject CharacterButton)
    {
        CharacterSelectionButtonInformation characterInformation = CharacterButton.GetComponentInChildren<CharacterSelectionButtonInformation>();
        SetCharacterInformationSkills(CharacterButton, characterInformation);
        SetCharacterInformationWeapon(CharacterButton, characterInformation);
    }

    private void SetCharacterInformationSkills(GameObject CharacterButton, CharacterSelectionButtonInformation characterInformation)
    {   
        characterNameText.text = characterInformation.CharacterName;   
        characterPassiveText.text = characterInformation.CharacterPassive;
        characterActiveText.text = characterInformation.CharacterActiveCooldown;
        characterActiveDescriptionText.text = characterInformation.CharacterActiveDescription;
    }

    private void SetCharacterInformationWeapon(GameObject CharacterButton, CharacterSelectionButtonInformation characterInformation)
    {
        weaponNameText.text = characterInformation.WeaponName;
        weaponStatsText.text = characterInformation.WeaponAttackSpeed;
        weaponSpecialsText.text = characterInformation.WeaponSpecialAbility;
    }
    
    private void ClearCharacterInformation()
    {
        characterNameText.text = "";   
        characterPassiveText.text = "";
        characterActiveText.text = "";
        characterActiveDescriptionText.text = "";
    
        weaponNameText.text = "";
        weaponStatsText.text = "";
        weaponSpecialsText.text = "";
    }


    private void ReturnToMenu()
    {
        TitleScreen.SetActive(true);
        this.gameObject.SetActive(false);
    }

    private void SelectCharacter()
    {
        DifficultySelectionPanel.SetActive(true);
        this.gameObject.SetActive(false);
    }
}
