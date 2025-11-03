using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] private GameObject Floor;
    [SerializeField] private GameObject CooldownAbilityOverlay;
    [SerializeField] private GameObject ActiveAbilityOverlay;
    [SerializeField] private Player player;

    public float currentWaveNumber = 0f;
    public bool isRoundOver = false;
    public Bounds mapSize;

    private void Start()
    {
        Transform FloorVisuals = Floor.transform.GetChild(0);
        Transform FloorSprite = FloorVisuals.GetChild(0);
        SpriteRenderer FloorRenderer = FloorSprite.GetComponent<SpriteRenderer>();
        mapSize = FloorRenderer.bounds;
    }

    private void OnEnable()
    {
        EnemySpawner.OnRoundOver += SetPlayerInactiveOnRoundOver;
    }

    private void OnDisable()
    {
        EnemySpawner.OnRoundOver -= SetPlayerInactiveOnRoundOver;
    }

    public void SetAbilityUIActive()
    {
        ActiveAbilityOverlay.SetActive(true);
    }

    public void SetAbilityUIInactive()
    {
        ActiveAbilityOverlay.SetActive(false);
    }

    public void StartAbilityCooldown()
    {
        CooldownAbilityOverlay.SetActive(true);
    }

    public void StopAbilityCooldown()
    {
        CooldownAbilityOverlay.SetActive(false);
    }

    private void SetPlayerInactiveOnRoundOver()
    {
        //player.gameObject.SetActive(false);
    }
}

