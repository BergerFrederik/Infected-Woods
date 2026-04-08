using UnityEngine;


public class Projectile : MonoBehaviour
{
    public WeaponStats sourceWeaponStats;
    [SerializeField] private WeaponStats weaponStats;
    private Vector3 startingPosition;
    private float distanceToTravel;
    private GameObject Player;
    private PlayerStats playerStats;
    
    private void Start()
    {
        Player = GameObject.FindGameObjectWithTag("Player");
        playerStats = Player.GetComponent<PlayerStats>();
        
        if (sourceWeaponStats != null && weaponStats != null)
        {
            weaponStats.CopyFrom(sourceWeaponStats);
        }
        
        startingPosition = this.transform.position;
        
        float playerAttackRange = playerStats.playerAttackRange;
        float weaponAttackRange = weaponStats.weaponRange; 
        distanceToTravel = weaponAttackRange * (1f + (playerAttackRange / 100f));
    }

    private void Update()
    {
        if (CalculateDistanceTraveled() < distanceToTravel)
        {
            this.transform.position += -(this.transform.up + this.transform.right) * sourceWeaponStats.weaponProjectileSpeed * Time.deltaTime; //Sprites m�ssen nach oben zeigen
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
