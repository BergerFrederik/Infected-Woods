using System.Runtime.CompilerServices;
using UnityEngine;

public class EnemyIsHitByPlayer : MonoBehaviour
{
    [SerializeField] private Transform popUpDamage;

    private void OnTriggerEnter2D(Collider2D collider)
    {
        if (collider.CompareTag("Projectile"))
        {
            if (collider.TryGetComponent<Projectile>(out Projectile projectileScript))
            {
                
            }
            InstantiatePopUpDamage();
        }                      
    }
    private void InstantiatePopUpDamage()
    {
        //Instantiate(popUpDamage, transform.position, Quaternion.identity);
    }
}
