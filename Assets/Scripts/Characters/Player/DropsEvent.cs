using UnityEngine;

public class DropsEvent : MonoBehaviour
{
    [SerializeField] private PlayerStats playerStats;
    [SerializeField] private Player player;
    [SerializeField] private float drop_floatspeed= 7f;

    private void Update()
    {
        PickupItems();
    }

    private void OnEnable()
    {
        player.OnDropCollected += ProcessDrop;
    }

    private void OnDisable()
    {
        player.OnDropCollected -= ProcessDrop;
    }
    private void PickupItems()
    {
        float baseRadius = playerStats.playerBasePickupRange;
        float bonusRadius = playerStats.playerLightPickupRange / 100;
        float searchRadius = baseRadius + baseRadius * bonusRadius;
        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, searchRadius);

        foreach (Collider2D collider in colliders)
        {
            if (collider.CompareTag("Drop"))
            {
                Vector3 directionToPlayer = player.transform.position - collider.transform.position;
                collider.transform.position += directionToPlayer.normalized * drop_floatspeed * Time.deltaTime;
            }
        }
    }

    private void ProcessDrop(Collider2D collider)
    {
        if (collider.TryGetComponent<LightDrop>(out LightDrop lightDrop))
        {
            Destroy(collider.gameObject);
            playerStats.playerLightAmount += lightDrop.lightDropValue;
        }       
    }
}
