using System.Collections;
using UnityEngine;

public class StompAttack : MonoBehaviour
{
    [SerializeField] private Pathfinder pathfinder;
    [SerializeField] private Collider2D bossCollider;
    [SerializeField] private EnemyJumpTowardsPlayer enemyJumpTowardsPlayer;
    [SerializeField] private BigLou bigLou;

    [SerializeField] private float stompAttackChargeTime;
    [SerializeField] private float stompAttackFloatTime;
    [SerializeField] private float stompJumpSpeed;

    public IEnumerator performStompAttack()
    {
        pathfinder.enabled = false;

        yield return new WaitForSeconds(stompAttackChargeTime);

        bossCollider.enabled = false;
        Vector3 targetY30 = new Vector3(transform.position.x, 30f, transform.position.z);

        while (Vector3.Distance(transform.position, targetY30) > 0.1f)
        {
            transform.position = Vector3.MoveTowards(transform.position, targetY30, stompJumpSpeed * Time.deltaTime);
            yield return null;
        }
        transform.position = targetY30;
        
        Vector2 playerPosition = pathfinder.GetPlayerPosition();
        float floatStartTime = Time.time;
        while (Time.time - floatStartTime <= stompAttackFloatTime)
        {
            playerPosition = pathfinder.GetPlayerPosition();
            transform.position = new Vector3(playerPosition.x, 30f, 0);
            yield return null;
        }
        
        while (Vector3.Distance(transform.position, playerPosition) > 0.1f)
        {
            transform.position = Vector3.MoveTowards(transform.position, playerPosition, stompJumpSpeed * Time.deltaTime);
            yield return null;
        }
    
        transform.position = playerPosition;
        bossCollider.enabled = true;
    
        pathfinder.enabled = true;
        bigLou.currentState = BigLou.BossState.Resting;
    }
}
