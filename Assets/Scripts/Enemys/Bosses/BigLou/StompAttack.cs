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
            float step = stompJumpSpeed * Time.deltaTime;
            this.transform.position = Vector3.MoveTowards(this.transform.position, targetY30, step);
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
        bigLou.currentState = BigLou.BossState.Resting;
    }
}
