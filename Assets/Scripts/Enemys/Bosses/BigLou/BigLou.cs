using UnityEngine;

public class BigLou : MonoBehaviour
{
    [SerializeField] private EnemyStats enemyStats;
    [SerializeField] private Pathfinder pathfinder;
    [SerializeField] private RollAttack rollAttack;
    [SerializeField] private StompAttack stompAttack;
    [SerializeField] private InfectionAttack infectionAttack;

    [SerializeField] private float stompAttackCooldown;
    [SerializeField] private float infectionAttackCooldown;
    [SerializeField] private float rollAttackCooldown;
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
                int randomAttack = Random.Range(2, 2);
                if (randomAttack == 0)
                {
                    StartCoroutine(stompAttack.performStompAttack());
                }
                else if (randomAttack == 1)
                {
                    StartCoroutine(rollAttack.PerformRollAttack());
                }
                else
                {
                    StartCoroutine(infectionAttack.PerformInfectionAttack());
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
}
