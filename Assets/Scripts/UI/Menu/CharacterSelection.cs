using System.Collections.Generic;
using TMPro;
using UnityEngine;
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
    
    [Header("UI")]
    [SerializeField] private TextMeshProUGUI characterNameText;
    [SerializeField] private TextMeshProUGUI characterPassiveText;
    [SerializeField] private TextMeshProUGUI characterActiveText;
    [SerializeField] private TextMeshProUGUI characterActiveDescriptionText;
    [SerializeField] private TextMeshProUGUI weaponNameText;
    [SerializeField] private TextMeshProUGUI weaponLoreText;
    [SerializeField] private TextMeshProUGUI weaponSpecialsText;
    [SerializeField] private Image characterBackgroundImage;
    [SerializeField] private Image weaponBackgroundImage;
    [SerializeField] private Image characterAbilityImage;
    
    private Dictionary<string, GameObject> _availableWeaponsMap = new Dictionary<string, GameObject>();

    private GameObject _preselectedCharacterButtonObject;
    private GameObject _preselectedCharacterPrefab;
    private GameObject _preselectedCharacterWeapon;
    
    public GameObject PreselectedCharacterPrefab => _preselectedCharacterPrefab;
    public GameObject PreselectedCharacterWeapon => _preselectedCharacterWeapon;

    private Color resetColor = new Color32(43, 43, 43, 255);
    

    private void OnEnable()
    {
        ResetPanel();
        returnButton.onClick.AddListener(() => ReturnToMenu());
        selectCharacterButton.onClick.AddListener(() => SelectCharacter());
        ClearCharacterInformation();
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
            newCharacterButtonObject.GetComponentInChildren<Button>().onClick.AddListener(() => PreselectCharacter(newCharacterButtonObject, character));
            OnHover onHover = newCharacterButtonObject.GetComponentInChildren<OnHover>();
            onHover.OnCursorHoverEnter += (obj) => DisplayCharacterInformation(newCharacterButtonObject);
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
        characterInformation.CharacterPassiveEffects = characterStats.characterPassiveEffects;
        characterInformation.CharacterActiveCooldown = characterStats.ability_cooldown.ToString();
        characterInformation.CharacterActiveDuration = characterStats.ability_duration.ToString();
        characterInformation.CharacterActiveManaCost = characterStats.ability_manaCost.ToString();
        characterInformation.CharacterActiveDescription = characterStats.abilityDescription;
        characterInformation.WeaponName = characterWeapon.name;
        characterInformation.WeaponLore = weaponStats.weaponLore;
        characterInformation.WeaponSpecialAbility= weaponStats.weaponSpecialAbility;
    }
    
    private void DisplayCharacterInformation(GameObject characterButtonObject)
    {
        CharacterSelectionButtonInformation characterInformation = characterButtonObject.GetComponentInChildren<CharacterSelectionButtonInformation>();
        SetCharacterInformationSkills(characterButtonObject, characterInformation);
        SetCharacterInformationWeapon(characterButtonObject, characterInformation);
        
        Button characterSelectionButton = characterButtonObject.GetComponentInChildren<Button>();
        characterBackgroundImage.sprite = characterSelectionButton.GetComponent<Image>().sprite;

        float midrangeImageAlphaValue = 0.15f;
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
        characterPassiveText.text = "";
        foreach (string information in characterInformation.CharacterPassiveEffects)
        {
            characterPassiveText.text += information + System.Environment.NewLine;
        }
        characterActiveText.text = $"Cooldown: {characterInformation.CharacterActiveCooldown}s\n" +
                                   $"Mana Cost: {characterInformation.CharacterActiveManaCost}\n" +
                                   $"Duration: {characterInformation.CharacterActiveDuration}s";
        characterActiveDescriptionText.text = characterInformation.CharacterActiveDescription;
    }


    private void SetCharacterInformationWeapon(GameObject CharacterButton, CharacterSelectionButtonInformation characterInformation)
    {
        weaponNameText.text = characterInformation.WeaponName;
        weaponLoreText.text = characterInformation.WeaponLore;
        weaponSpecialsText.text = characterInformation.WeaponSpecialAbility;
    }
    
    
    private void ClearCharacterInformation()
    {
        if (_preselectedCharacterButtonObject == null)
        {
            characterNameText.text = "";   
            characterPassiveText.text = "";
            characterActiveText.text = "";
            characterActiveDescriptionText.text = "";
    
            weaponNameText.text = "";
            weaponLoreText.text = "";
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
            DisplayCharacterInformation(_preselectedCharacterButtonObject);
        }
        
    }

    private void ResetPanel()
    {
        if (_preselectedCharacterButtonObject == null)
        {
            return;
        }
        HandleHighlightButton(_preselectedCharacterButtonObject, resetColor);
        _preselectedCharacterButtonObject = null;
        _preselectedCharacterWeapon = null;
        _preselectedCharacterPrefab = null;
    }
    
    private void SetImageAlpha(Image image, float alpha)
    {
        Color transparentImage = image.color;
        transparentImage.a = alpha;
        image.color = transparentImage;
    }

    private void PreselectCharacter(GameObject characterButtonObject, GameObject character)
    {
        if (_preselectedCharacterButtonObject != characterButtonObject)
        {
            if (_preselectedCharacterButtonObject != null)
            {
                HandleHighlightButton(_preselectedCharacterButtonObject, resetColor);    
            }
            HandleHighlightButton(characterButtonObject, Color.white);
            SetPreselectedGameObject(characterButtonObject, character);
        }
        else
        {
            HandleHighlightButton(characterButtonObject, resetColor);
            SetPreselectedGameObject(null, null);  
        }
    }

    private void HandleHighlightButton(GameObject characterButtonObject, Color color)
    {
        characterButtonObject.GetComponentInChildren<Image>().color = color;
    }

    private void SetPreselectedGameObject(GameObject characterButtonObject, GameObject character)
    {
        _preselectedCharacterButtonObject = characterButtonObject;
        _preselectedCharacterPrefab = character;
    }
    private void ReturnToMenu()
    {
        TitleScreen.SetActive(true);
        this.gameObject.SetActive(false);
    }

    private void SelectCharacter()
    {
        if (_preselectedCharacterButtonObject == null)
        {
            return;
        }

        string characterWeaponName = _preselectedCharacterPrefab.GetComponent<CharacterStats>().startWeaponID;
        _preselectedCharacterWeapon = _availableWeaponsMap[characterWeaponName];
        
        DifficultySelectionPanel.SetActive(true);
        this.gameObject.SetActive(false);
    }
}
