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
    [SerializeField] private GameObject CharacterButtonPrefab;

    private void OnEnable()
    {
        returnButton.onClick.AddListener(() => ReturnToMenu());
        selectCharacterButton.onClick.AddListener(() => SelectCharacter());
        
        LoadCharacters();
    }

    private void OnDisable()
    {
        returnButton.onClick.RemoveAllListeners();
        selectCharacterButton.onClick.RemoveAllListeners();
    }

    private void LoadCharacters()
    {
        foreach (GameObject character in PlayerCharacters)
        {
            GameObject newCharacterButtonObject = Instantiate(CharacterButtonPrefab,  PlayerCharacterList.transform);
            newCharacterButtonObject.transform.GetChild(0).GetChild(0).GetComponent<Image>().sprite = character.GetComponentInChildren<SpriteRenderer>().sprite;
        }
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
