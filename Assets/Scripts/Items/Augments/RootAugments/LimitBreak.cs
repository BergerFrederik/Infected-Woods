using UnityEngine;

// Increase the stack limit of all stackables
// The bonus lives on PlayerStats and is read by StackCounter, so other sources can raise it the same way.

public class LimitBreak : MonoBehaviour, IAugmentDescribable
{
    [SerializeField] private float stackIncreasePercentage; // value between 0 and 100

    private Transform _player;
    private PlayerStats _playerStats;

    private void Start()
    {
        _player = this.transform.root;
        _playerStats = _player.GetComponent<PlayerStats>();

        PerformAugment();
    }

    private void OnDestroy()
    {
        if (_playerStats != null)
        {
            _playerStats.playerStackCapIncreasePercent -= stackIncreasePercentage;
        }
    }

    private void PerformAugment()
    {
        _playerStats.playerStackCapIncreasePercent += stackIncreasePercentage;
    }

    public float GetPlaceholderValue(int index)
    {
        return index switch
        {
            0 => stackIncreasePercentage,
            _ => 0f
        };
    }
}
