using UnityEngine;


public class Projectile : MonoBehaviour
{
    public WeaponStats sourceWeaponStats;
    [SerializeField] private WeaponStats weaponStats;
    private Vector3 startingPosition;
    private float distanceToTravel;
    private GameObject Player;
    private PlayerStats playerStats;
    private Transform _target;
    private bool _hadTarget;

    // Set for homing projectiles (e.g. Valmidir's ability). Also remembers that there was a
    // target, so a projectile whose target died doesn't turn into one that hits anything.
    public Transform target
    {
        get => _target;
        set
        {
            _target = value;
            _hadTarget = value != null;
        }
    }

    // Homing projectiles may only hit the enemy they fly towards and pass through all others.
    // Projectiles without a target (arrows) can hit any enemy.
    public bool CanHit(Transform enemy) => !_hadTarget || enemy == _target;

    private void Start()
    {
        Player = GameObject.FindGameObjectWithTag("Player");
        playerStats = Player.GetComponent<PlayerStats>();

        if (sourceWeaponStats != null && weaponStats != null)
        {
            weaponStats.CopyFrom(sourceWeaponStats);
        }

        startingPosition = this.transform.position;

        float weaponAttackRange = weaponStats.weaponRange;
        distanceToTravel = weaponAttackRange * playerStats.GetAttackRangeFactor();
    }

    private void Update()
    {
        if (_hadTarget)
        {
            if (target == null)
            {
                Destroy(this.gameObject);
                return;
            }
            transform.position += (target.position - transform.position).normalized * sourceWeaponStats.weaponProjectileSpeed * Time.deltaTime;
        }
        else if (CalculateDistanceTraveled() < distanceToTravel)
        {
            this.transform.position += -(this.transform.up + this.transform.right).normalized * sourceWeaponStats.weaponProjectileSpeed * Time.deltaTime; //Sprites m�ssen nach oben zeigen
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
