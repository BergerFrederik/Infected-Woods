using UnityEngine;

// Gain x% more stacks
// The bonus lives on PlayerStats and is read by StackCounter, so other sources can raise it the same way.

public class SkyKamiReach : MonoBehaviour, IAugmentDescribable
{
    [SerializeField] private float percentStacksIncrease = 50f; // num in percent

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
            _playerStats.playerStackGainIncreasePercent -= percentStacksIncrease;
        }
    }

    private void PerformAugment()
    {
        _playerStats.playerStackGainIncreasePercent += percentStacksIncrease;
    }

    public float GetPlaceholderValue(int index)
    {
        return index switch
        {
            0 => percentStacksIncrease,
            _ => 0f
        };
    }
}
