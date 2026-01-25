using System;
using UnityEngine;
using UnityEngine.UI;

public class DifficultySelection : MonoBehaviour
{
    [SerializeField] private Button[] DifficultyButtons;
    [SerializeField] private Button returnButton;
    [SerializeField] private GameObject CharacterSelectionPanel;
    [SerializeField] private GameObject MainMenu;

    public event Action<int> OnDifficultySelected;

    private void OnEnable()
    {
        for (int i = 0; i < DifficultyButtons.Length; i++)
        {
            int index = i;
            DifficultyButtons[i].onClick.AddListener(() => SelectDifficulty(index));
        }
        returnButton.onClick.AddListener(() => ReturnToCharacterSelection());
    }

    private void OnDisable()
    {
        foreach (Button button in DifficultyButtons)
        {
            button.onClick.RemoveAllListeners();
        } 
        returnButton?.onClick.RemoveAllListeners();
    }

    private void SelectDifficulty(int index)
    {
        foreach (Button button in DifficultyButtons)
        {
            button.onClick.RemoveAllListeners();
        } 
        OnDifficultySelected?.Invoke(index);
        this.gameObject.SetActive(false);
        MainMenu.SetActive(false);
    }

    private void ReturnToCharacterSelection()
    {
        CharacterSelectionPanel.SetActive(true);
        this.gameObject.SetActive(false);
    }
}
