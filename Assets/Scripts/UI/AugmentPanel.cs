
using System.Collections.Generic;

using TMPro;
using UnityEngine;

using UnityEngine.UI;



public class AugmentPanel : MonoBehaviour
{
    [SerializeField] private GameObject levelPanel;
    [SerializeField] private GameObject shopPanel;
    [SerializeField] private Player player;
    [SerializeField] private PlayerStats playerStats;

    public List<GameObject> AugmentItems;
    public Sprite[] AugmentSprites;
    public Button[] AugmentButtons;
    public GameObject[] AugmentTitles;
    public GameObject[] AugmentContents;

    private GameObject[] ChosenAugments;
    private GameObject LastAugment = null;
    private Dictionary<string, Sprite> AugmentSpriteMap;

    private GameObject playerAugments;

    private void Awake()
    {
        AugmentSpriteMap = new Dictionary<string, Sprite>();
        foreach (Sprite sprite in AugmentSprites)
        {
            AugmentSpriteMap.Add(sprite.name, sprite);
        }
        Transform playerAugmentsTransform = player.transform.Find("PlayerAugments");
        playerAugments = playerAugmentsTransform.gameObject;
    }

    private void OnEnable()
    {
        string augmentRarity = DetermineAugmentRarity();
        DetermineAugmentsToChooseFrom(augmentRarity);
        SetSpritesToButtons();
    }

    private string DetermineAugmentRarity()
    {
        float chance_to_get_bud = 30;
        float chance_to_get_blossom = 20;
        float randomNumber = Random.Range(0f, 100f);
        if (randomNumber <= chance_to_get_blossom)
        {
            return "Blossom";
        }
        else if (randomNumber > chance_to_get_blossom && randomNumber <= chance_to_get_bud + chance_to_get_blossom)
        {
            return "Bud";
        }
        else
        {
            return "Root";
        }
    }

    private void DetermineAugmentsToChooseFrom(string augmentRarity)
    {
        List<GameObject> augmentsToChooseFrom = new List<GameObject>();
        foreach (GameObject augment in AugmentItems)
        {
            if (augment.CompareTag(augmentRarity))
            {
                augmentsToChooseFrom.Add(augment);
            }
        }
        ChooseAugments(augmentsToChooseFrom);
    }

    private void ChooseAugments(List<GameObject> AugmentsToChooseFrom)
    {
        int amount_of_augments_to_choose_from = 3;
        ChosenAugments = new GameObject[amount_of_augments_to_choose_from];
        for (int i = 0; i < amount_of_augments_to_choose_from; i++)
        {
            if (AugmentsToChooseFrom.Count > 0)
            {
                int randomIndex = Random.Range(0, AugmentsToChooseFrom.Count);
                GameObject chosenAugment = AugmentsToChooseFrom[randomIndex];
                ChosenAugments[i] = chosenAugment;
                AugmentItems.Remove(chosenAugment);
                AugmentsToChooseFrom.RemoveAt(randomIndex);
                LastAugment = chosenAugment;                              
            }
            else
            {
                ChosenAugments[i] = LastAugment;
            }           
        }
    }
    private void SetSpritesToButtons()
    {
        for (int i = 0; i < ChosenAugments.Length; i++)
        {
            string augmentText = ChosenAugments[i].gameObject.GetComponent<AugmentInformation>().augmentText;
            string ID = ChosenAugments[i].GetComponent<AugmentInformation>().augmentID;
            Sprite AugmentButtonSprite = AugmentSpriteMap[ID];
            AugmentButtons[i].GetComponent<Image>().sprite = AugmentButtonSprite;
            AugmentTitles[i].GetComponent<TextMeshProUGUI>().text = ID;
            AugmentContents[i].GetComponent<TextMeshProUGUI>().text = augmentText;


            AugmentButtons[i].onClick.RemoveAllListeners();

            int index = i;
            AugmentButtons[i].onClick.AddListener(() => SelectAugment(index));
        }
    }
    public void SelectAugment(int buttonIndex)
    {
        GameObject ChosenAugmentPrefab = ChosenAugments[buttonIndex];
        GameObject NewAugment = Instantiate(ChosenAugmentPrefab);
        NewAugment.transform.SetParent(playerAugments.transform, false);
        if (playerStats.playerLeveledUp)
        {
            levelPanel.SetActive(true);
        }
        else
        {
            shopPanel.SetActive(true);
        }
        this.gameObject.SetActive(false);
    }
}
