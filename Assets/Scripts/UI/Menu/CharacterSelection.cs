using UnityEngine;
using UnityEngine.UI;

public class CharacterSelection : MonoBehaviour
{
    [SerializeField] private Button returnButton;
    [SerializeField] private Button selectCharacterButton;
    [SerializeField] private GameObject DifficultySelectionPanel;
    [SerializeField] private GameObject TitleScreen;

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
