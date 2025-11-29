using UnityEngine;
using UnityEngine.UIElements;

public class Projectile : MonoBehaviour
{
    [HideInInspector] public WeaponStats sourceWeaponStats;

    private Vector3 startingPosition;
    private float distanceToTravel;
    private GameObject Player;
    private PlayerStats playerStats;
    private void Start()
    {
        Player = GameObject.FindGameObjectWithTag("Player");
        playerStats = Player.GetComponent<PlayerStats>();
        startingPosition = this.transform.position;
        float playerAttackRange = playerStats.playerAttackRange;
        float weaponAttackRange = sourceWeaponStats.weaponRange; 
        distanceToTravel = weaponAttackRange + weaponAttackRange * playerAttackRange;
    }

    private void Update()
    {
        if (CalculateDistanceTraveled() < distanceToTravel)
        {
            this.transform.position += -(this.transform.up + this.transform.right) * sourceWeaponStats.weaponProjectileSpeed * Time.deltaTime; //Sprites müssen nach oben zeigen
        }
        else 
        {
            Destroy(this.gameObject);
        }
    }

    private float CalculateDistanceTraveled()
    {
        Vector3 currentPosition = transform.position;
        float distanceTraveled = Vector3.Distance(startingPosition, currentPosition);
        return distanceTraveled;
    }
    // bounces einbauen
    private void OnTriggerEnter2D(Collider2D collider)
    {
        // if darf bounce
        //{
        //    calculateBounceRichutng();
        //}
        if (collider.gameObject.CompareTag("Floor"))
        {
            Destroy(this.gameObject);
        }
    }
}
