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
    [SerializeField] private Image characterBackgroundImage;
    [SerializeField] private Image weaponBackgroundImage;
    [SerializeField] private Image characterAbilityImage;
    
    private Dictionary<string, GameObject> _availableWeaponsMap = new Dictionary<string, GameObject>();

    private GameObject preselectedCharacter;

    private void OnEnable()
    {
        returnButton.onClick.AddListener(() => ReturnToMenu());
        selectCharacterButton.onClick.AddListener(() => SelectCharacter());
        ClearCharacterInformation();
    }
    
    private void OnDisable()
    {
        returnButton.onClick.RemoveAllListeners();
        selectCharacterButton.onClick.RemoveAllListeners();

        preselectedCharacter = null;
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
            newCharacterButtonObject.GetComponentInChildren<Button>().onClick.AddListener(() => PreselectCharacter(newCharacterButtonObject));
            OnHover onHover = newCharacterButtonObject.GetComponentInChildren<OnHover>();
            onHover.OnCursorHoverEnter += DisplayCharacterInformation;
            onHover.OnCursorHoverExit += ClearCharacterInformation;
            SetInformationToButton(character, newCharacterButtonObject);
        }
    }

    private void SetInformationToButton(GameObject character, GameObject CharacterButtonObject)
    {
        CharacterSelectionButtonInformation characterInformation = CharacterButtonObject.GetComponentInChildren<CharacterSelectionButtonInformation>();
        CharacterStats characterStats = character.GetComponentInChildren<CharacterStats>();
        GameObject characterWeapon = _availableWeaponsMap[characterStats.startWeaponID];
        WeaponStats weaponStats = characterWeapon.GetComponent<WeaponStats>();
        GameObject ButtonObject = CharacterButtonObject.GetComponentInChildren<Button>().gameObject;
        Button newCharacterButton = CharacterButtonObject.GetComponentInChildren<Button>();
        GameObject buttonAbilityImageGameobject = ButtonObject.transform.Find("CharacterAbilityImage").gameObject;
        Image characterAbilityImageOnButton = buttonAbilityImageGameobject.GetComponentInChildren<Image>();
        GameObject characterUIVisuals = character.transform.Find("UIVisuals").gameObject;
        
        newCharacterButton.GetComponent<Image>().sprite = character.GetComponentInChildren<SpriteRenderer>().sprite;
        characterAbilityImageOnButton.sprite = characterUIVisuals.GetComponentInChildren<SpriteRenderer>().sprite;
        
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
    
    private void DisplayCharacterInformation(GameObject characterButtonObject)
    {
        CharacterSelectionButtonInformation characterInformation = characterButtonObject.GetComponentInChildren<CharacterSelectionButtonInformation>();
        SetCharacterInformationSkills(characterButtonObject, characterInformation);
        SetCharacterInformationWeapon(characterButtonObject, characterInformation);
        
        Button characterSelectionButton = characterButtonObject.GetComponentInChildren<Button>();
        characterBackgroundImage.sprite = characterSelectionButton.GetComponent<Image>().sprite;

        float midrangeImageAlphaValue = 0.5f;
        SetImageAlpha(characterBackgroundImage, midrangeImageAlphaValue);
        
        GameObject characterWeapon = _availableWeaponsMap[characterInformation.WeaponName];
        weaponBackgroundImage.sprite = characterWeapon.GetComponentInChildren<SpriteRenderer>().sprite;
        SetImageAlpha(weaponBackgroundImage, midrangeImageAlphaValue);
        
        characterAbilityImage.sprite = characterSelectionButton.gameObject.transform.GetChild(0).GetComponent<Image>().sprite;
        SetImageAlpha(characterAbilityImage, 1f);
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
        if (preselectedCharacter == null)
        {
            characterNameText.text = "";   
            characterPassiveText.text = "";
            characterActiveText.text = "";
            characterActiveDescriptionText.text = "";
    
            weaponNameText.text = "";
            weaponStatsText.text = "";
            weaponSpecialsText.text = "";

            float transparentImageAlphaValue = 0f;
            characterBackgroundImage.sprite = null;
            SetImageAlpha(characterBackgroundImage, transparentImageAlphaValue);
            weaponBackgroundImage.sprite = null;
            SetImageAlpha(weaponBackgroundImage, transparentImageAlphaValue);
            characterAbilityImage.sprite = null;
            SetImageAlpha(characterAbilityImage, transparentImageAlphaValue);
        }

        else
        {
            DisplayCharacterInformation(preselectedCharacter);
        }
        
    }
    
    private void SetImageAlpha(Image image, float alpha)
    {
        Color transparentImage = image.color;
        transparentImage.a = alpha;
        image.color = transparentImage;
    }

    private void PreselectCharacter(GameObject characterObject)
    {
        //Highlight button Object
        //setHoverOnDefault
        preselectedCharacter = characterObject;

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
