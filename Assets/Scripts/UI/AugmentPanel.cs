using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;



public class AugmentPanel : MonoBehaviour
{
    [SerializeField] Player player;
    [SerializeField] GameObject PlayerAugments;
    [SerializeField] private GameManager gameManager;

    public List<GameObject> AugmentItems;
    public Button[] AugmentButtons;
    public GameObject[] AugmentTitles;
    public GameObject[] AugmentContents;
    public GameObject[] AugmentIcons;
    
    public float chance_to_get_bud = 30;
    public float chance_to_get_blossom = 20;

    private GameObject[] ChosenAugments;
    private GameObject LastAugment = null;

    private void OnEnable()
    {
        int augmentRarity = DetermineAugmentRarity();
        DetermineAugmentsToChooseFrom(augmentRarity);
        SetSpritesToButtons();
    }

    private int DetermineAugmentRarity()
    {
        
        float randomNumber = Random.Range(0, 100);
        if (randomNumber <= chance_to_get_blossom)
        {
            return 2;
        }
        else if (randomNumber > chance_to_get_blossom && randomNumber <= chance_to_get_bud + chance_to_get_blossom)
        {
            return 1;
        }
        else
        {
            return 0;
        }
    }

    private void DetermineAugmentsToChooseFrom(int augmentRarity)
    {
        List<GameObject> augmentsToChooseFrom = new List<GameObject>();
        foreach (GameObject augment in AugmentItems)
        {
            if (augment.GetComponent<AugmentInformation>().augmentRarity == augmentRarity)
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
            AugmentInformation augmentInformation = ChosenAugments[i].gameObject.GetComponent<AugmentInformation>();
            string augmentText = augmentInformation.augmentText;
            string augmentTitle = augmentInformation.augmentTitle;

            Transform AugmentVisuals = ChosenAugments[i].transform.GetChild(0);
            GameObject AugmentSpiteObject = AugmentVisuals.Find("Sprite").gameObject;
            GameObject AugmentIconObject = AugmentVisuals.Find("Icon").gameObject;

            Sprite AugmentButtonSprite = AugmentSpiteObject.GetComponent<Image>().sprite;
            Sprite AugmentButtonIcon = AugmentIconObject.GetComponent<Image>().sprite;

            AugmentButtons[i].GetComponent<Image>().sprite = AugmentButtonSprite;
            AugmentTitles[i].GetComponent<TextMeshProUGUI>().text = augmentTitle;
            AugmentContents[i].GetComponent<TextMeshProUGUI>().text = augmentText;
            AugmentIcons[i].GetComponent<Image>().sprite = AugmentButtonIcon;


            AugmentButtons[i].onClick.RemoveAllListeners();

            int index = i;
            AugmentButtons[i].onClick.AddListener(() => SelectAugment(index));
        }
    }
    public void SelectAugment(int buttonIndex)
    {
        GameObject ChosenAugmentPrefab = ChosenAugments[buttonIndex];
        GameObject NewAugment = Instantiate(ChosenAugmentPrefab);
        NewAugment.transform.SetParent(PlayerAugments.transform, false);
        StartCoroutine(WaitForAugmentsToLoadReferences());
    }

    private IEnumerator WaitForAugmentsToLoadReferences() // we need this function to load references on the Augments
    {
        Time.timeScale = 1.0f;
        yield return new WaitForSeconds(0.1f);
        gameManager.CycleShops();
        this.gameObject.SetActive(false);
    }
}
