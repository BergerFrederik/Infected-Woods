using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class BigLou : MonoBehaviour
{
    [SerializeField] private EnemyStats enemyStats;
    [SerializeField] private Pathfinder pathfinder;
    [SerializeField] private EnemyJumpTowardsPlayer enemyJumpTowardsPlayer;
    [SerializeField] private Collider2D bossCollider;
    [SerializeField] private float stompAttackCooldown;
    [SerializeField] private float infectionAttackCooldown;
    [SerializeField] private float rollAttackCooldown;

    [SerializeField] private float stompDamage;
    [SerializeField] private float infectionDamage;
    [SerializeField] private float rollDamage;

    [SerializeField] private float stompAttackChargeTime;
    [SerializeField] private float stompAttackFloatTime;
    [SerializeField] private float stompJumpSpeed;

    [SerializeField] private float rollDamageMultiplier;
    [SerializeField] private float restTime;

    [SerializeField] private int minCooldown;
    [SerializeField] private int maxCooldown;

    private float restingStartTime;
    private float cooldownStartTime;
    private float randomCooldown;

    public enum BossState
    {
        Resting,
        Walking,
        PrepareAttack,
        Attacking
    }

    public BossState currentState = BossState.Resting;

    private void Start()
    {
        restingStartTime = Time.time - restTime;
    }
    private void Update()
    {
        HandleStates();
    }

    private void HandleStates()
    {
        switch (currentState)
        {
            case BossState.Resting:
                if (Time.time - restingStartTime >= restTime)
                {
                    currentState = BossState.Walking;
                }
                break;
            case BossState.Walking:
                if (randomCooldown == 0)
                {
                    int randomNumber = Random.Range(minCooldown, maxCooldown);
                    randomCooldown = randomNumber;
                    cooldownStartTime = Time.time;
                }
                if (Time.time - cooldownStartTime >= randomCooldown)
                {                 
                    currentState = BossState.PrepareAttack;
                    randomCooldown = 0;
                }
                FollowPlayer();
                break;
            case BossState.PrepareAttack:
                int randomAttack = 0;
                if (randomAttack == 0)
                {
                    StartCoroutine(StompAttack());
                }
                else if (randomAttack == 1)
                {
                    StartCoroutine(RollAttack());
                }
                else
                {
                    StartCoroutine(InfectionAttack());
                }
                currentState = BossState.Attacking;
                break;
            case BossState.Attacking:
                break;
        }
    }


    private void FollowPlayer()
    {
        Vector2 moveDir = pathfinder.CalculateEnemyMovementVector();
        transform.position += (Vector3)moveDir * enemyStats.enemyMoveSpeed * Time.deltaTime;
    }

    private IEnumerator StompAttack()
    {
        // disable pathfinder to get him out of bounds
        pathfinder.enabled = false;

        
        
        // charge the jump
        yield return new WaitForSeconds(stompAttackChargeTime);

        // jump in the air
        bossCollider.enabled = false;
        Vector3 targetY30 = new Vector3(this.transform.position.x, 30f, this.transform.position.z);

        // Die Schleife läuft, solange die aktuelle Position nicht exakt die Zielposition ist.
        while (this.transform.position != targetY30)
        {
            // Berechne den Schritt, den wir in diesem Frame machen dürfen
            float step = stompJumpSpeed * Time.deltaTime;

            // Bewege die Position sicher in Richtung des Ziels.
            this.transform.position = Vector3.MoveTowards(this.transform.position, targetY30, step);

            // Pausiere bis zum nächsten Frame
            yield return null;
        }


        // float in the air and create landing circle
        Vector2 targetPosition = enemyJumpTowardsPlayer.CalculateTargetPosition();
        Vector3 floatPos = new Vector3(targetPosition.x, transform.position.y, 0);
        transform.position = floatPos;
        yield return new WaitForSeconds(stompAttackFloatTime);

        // execute stomp attack
        bossCollider.enabled = true;
        while (transform.position != (Vector3)targetPosition)
        {
            transform.position += new Vector3(0, -1, 0) * stompJumpSpeed * Time.deltaTime;
            if (transform.position.y <= targetPosition.y + 0.2f)
            {
                transform.position = targetPosition;
            }
            yield return null;
        }
        pathfinder.enabled = true;
        currentState = BossState.Resting;
    }

    private IEnumerator RollAttack()
    {
        currentState = BossState.Resting;
        yield return null;
    }

    private IEnumerator InfectionAttack()
    {
        currentState = BossState.Resting;
        yield return null;
    }
}
