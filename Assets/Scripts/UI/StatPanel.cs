using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class StatPanel : MonoBehaviour
{
    [SerializeField] private Button toggleButton;
    [SerializeField] private PlayerStats playerStats;
    
    [Header("Stats")]
    [SerializeField] private TextMeshProUGUI primaryStatsTextField;
    [SerializeField] private TextMeshProUGUI primaryValuesTextField;
    [SerializeField] private TextMeshProUGUI secondaryStatsTextField;
    [SerializeField] private TextMeshProUGUI secondaryValuesTextField;
    
    [Header("Augemnts")]
    [SerializeField] private Image[] augmentSlots;
    [SerializeField] private GameObject playerAugments;
    
    
    private void OnEnable()
    {
        toggleButton.onClick.AddListener(TogglePanelOff);
        StaticHelpers.SetPrimaryStatsToTextUI(primaryStatsTextField, primaryValuesTextField, playerStats);
        StaticHelpers.SetSecondaryStatsToTextUI(secondaryStatsTextField, secondaryValuesTextField, playerStats);

        SetAugmentImages();
    }

    private void OnDisable()
    {
        toggleButton.onClick.RemoveAllListeners();
    }

    private void SetAugmentImages()
    {
        for (int i = 0; i < playerAugments.transform.childCount; i++)
        {
            GameObject currentAugment = playerAugments.transform.GetChild(i).gameObject;
            AugmentInformation augmentInformation = currentAugment.GetComponent<AugmentInformation>();
            string augmentText = augmentInformation.augmentText;
            string augmentTitle = augmentInformation.augmentTitle;

            Transform augmentVisuals = currentAugment.transform.GetChild(0);
            GameObject augmentSpriteObject = augmentVisuals.Find("Sprite").gameObject;
            GameObject augmentIconObject = augmentVisuals.Find("Icon").gameObject;

            Sprite augmentButtonSprite = augmentSpriteObject.GetComponent<Image>().sprite;
            Sprite augmentButtonIcon = augmentIconObject.GetComponent<Image>().sprite;

            augmentSlots[i].GetComponent<Image>().sprite = augmentButtonSprite;
            //AugmentTitles[i].GetComponent<TextMeshProUGUI>().text = augmentTitle;
            //AugmentContents[i].GetComponent<TextMeshProUGUI>().text = augmentText;
            //AugmentIcons[i].GetComponent<Image>().sprite = AugmentButtonIcon;
        }
    }
    
    private void TogglePanelOff()
    {
        this.gameObject.SetActive(false);
    }
}
