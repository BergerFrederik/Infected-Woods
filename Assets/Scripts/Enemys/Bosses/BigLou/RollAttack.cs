using System.Collections;
using UnityEngine;


public class RollAttack : MonoBehaviour
{
    [SerializeField] private Pathfinder pathfinder;
    [SerializeField] private BigLou bigLou;

    [SerializeField] private float rollAcceleratorTime;
    [SerializeField] private float rollAttackDuration;
    [SerializeField] private float rollSpeed;
    [SerializeField] private float rollBoostPerHit;

    private Vector3 rollMoveDir;

    public IEnumerator PerformRollAttack()
    {
        //accelerate roll
        yield return new WaitForSeconds(rollAcceleratorTime);

        Vector3 playerPosition = pathfinder.GetPlayerPosition();
        pathfinder.enabled = false;
        rollMoveDir = (playerPosition - transform.position).normalized;
        float rollAttackStartTime = Time.time;
        while (Time.time - rollAttackStartTime <= rollAttackDuration)
        {
            transform.position += rollMoveDir * rollSpeed * Time.deltaTime;
            yield return null;
        }
        bigLou.currentState = BigLou.BossState.Resting;
        pathfinder.enabled = true;
        yield return null;
    }

    private void OnTriggerEnter2D(Collider2D collider)
    {
        if (collider.gameObject.CompareTag("Floor"))
        {
            rollSpeed += rollSpeed * rollBoostPerHit;
            Collider2D bossCol = GetComponent<Collider2D>();
            if (bossCol == null) return;
            Vector2 closestPointOnWall = collider.ClosestPoint(bossCol.transform.position);
            float xDiff = Mathf.Abs(closestPointOnWall.x - bossCol.transform.position.x);
            float yDiff = Mathf.Abs(closestPointOnWall.y - bossCol.transform.position.y);
            Vector2 calculatedNormal;
            if (xDiff > yDiff)
            {
                calculatedNormal = new Vector2(
                    bossCol.transform.position.x > closestPointOnWall.x ? 1f : -1f,
                    0f
                );
            }
            else
            {
                calculatedNormal = new Vector2(
                    0f,
                    bossCol.transform.position.y > closestPointOnWall.y ? 1f : -1f
                );
            }
            Vector3 reflectedDir = Vector2.Reflect(rollMoveDir, calculatedNormal);
            rollMoveDir = reflectedDir.normalized;
            transform.position += (Vector3)rollMoveDir * 0.1f;
            rollMoveDir = rollMoveDir.normalized;
        }
    }
}
