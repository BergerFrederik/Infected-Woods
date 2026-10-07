using UnityEngine;

public class DropsEvent : MonoBehaviour
{
    [SerializeField] private PlayerStats playerStats;
    [SerializeField] private Player player;
    [SerializeField] private CircleCollider2D pickupCollider;

    private void Start()
    {
        AlterColliderRadius(playerStats.playerLightPickupRange);
    }

    private void OnEnable()
    {
        playerStats.OnLightPickupRangeChanged += AlterColliderRadius;
    }

    private void OnDisable()
    {
        playerStats.OnLightPickupRangeChanged -= AlterColliderRadius;
    }

    private void OnTriggerEnter2D(Collider2D collider)
    {
        PickupItems(collider);
    }
    private void PickupItems(Collider2D collider)
    {
        if (collider.CompareTag("Drop"))
        {
            if (collider.TryGetComponent<DropBehaviour>(out DropBehaviour drop))
            {
                drop.CollectDrop(this.transform.root);
            }
        }
    }

    private void AlterColliderRadius(float bonusPickupRange)
    {
        float baseRadius = playerStats.playerBasePickupRange;
        float bonusRadius = bonusPickupRange / 100f;
        // A very negative pickup range must not give the collider a negative radius
        pickupCollider.radius = Mathf.Max(0f, baseRadius + bonusRadius);
    }
}
