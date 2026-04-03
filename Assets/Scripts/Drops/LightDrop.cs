using System.Collections;
using UnityEngine;

public class LightDrop : MonoBehaviour
{
    public float lightDropValue = 0f;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.TryGetComponent(out PlayerStats playerStats))
        {
            playerStats.playerLightAmount += lightDropValue;
            Destroy(this.gameObject);
        }
    }
}
