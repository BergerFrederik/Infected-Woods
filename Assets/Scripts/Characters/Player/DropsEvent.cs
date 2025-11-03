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

    private void PickupItems()
    {
        float searchRadius = playerStats.playerLightAbsorption;
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

    public void GainMoney(Collider2D collider)
    {
        if (collider.CompareTag("Drop"))
        {
            if (collider.TryGetComponent<LightDrop>(out LightDrop lightDrop))
            {
                Destroy(collider.gameObject);
                playerStats.playerLightAmount += lightDrop.lightDropValue;
            }
        }
    }
}
