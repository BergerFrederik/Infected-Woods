using System.Collections;
using UnityEngine;

public class EnemyJumpTowardsPlayer : MonoBehaviour
{
    [SerializeField] private float maxJumpDistance = 2f;
    [SerializeField] private float jumpDuration = 1f;
    [SerializeField] private float jumpHeight = 3f;
    [SerializeField] private float jumpCooldown = 2f;
    [SerializeField] private Pathfinder pathfinder;
    [SerializeField] private EnemyKnockback knockback;
    [SerializeField] private Collider2D enemyCollider;

    private float restStartTime;
    public enum JumpState { Resting, Preparing, Jumping }
    public JumpState currentState = JumpState.Resting;

    private void Start()
    {
        restStartTime = Time.time - jumpCooldown;
    }

    private void Update()
    {
        if (currentState == JumpState.Resting)
        {
            knockback.canReceiveKnockback = true;
            knockback.useLerpResistance = false; // Steht nur da, kein "Aufraffen"
            knockback.ApplyMovement(Vector3.zero);

            if (Time.time - restStartTime >= jumpCooldown)
                currentState = JumpState.Preparing;
        }
        else if (currentState == JumpState.Preparing)
        {
            Vector2 start = transform.position;
            Vector2 end = CalculateTargetPosition();
            StartCoroutine(JumpArc(start, end, jumpDuration, jumpHeight));
        }
    }

    private IEnumerator JumpArc(Vector2 start, Vector2 end, float duration, float height)
    {
        currentState = JumpState.Jumping;
        knockback.canReceiveKnockback = false; // Im Flug kein Knockback
        enemyCollider.enabled = false;
        pathfinder.enabled = false;

        float timeElapsed = 0f;
        while (timeElapsed < duration)
        {
            float t = timeElapsed / duration;
            Vector2 horizontal = Vector2.Lerp(start, end, t);
            float arc = height * Mathf.Sin(t * Mathf.PI);
            transform.position = new Vector3(horizontal.x, horizontal.y + arc, transform.position.z);
            timeElapsed += Time.deltaTime;
            yield return null;
        }

        transform.position = new Vector3(end.x, end.y, transform.position.z);
        pathfinder.enabled = true;
        enemyCollider.enabled = true;
        restStartTime = Time.time;
        currentState = JumpState.Resting;
    }

    public Vector2 CalculateTargetPosition()
    {
        Vector2 playerPos = pathfinder.GetPlayerPosition();
        Vector2 currentPos = transform.position;
        Vector2 dir = playerPos - currentPos;
        return dir.magnitude <= maxJumpDistance ? playerPos : currentPos + dir.normalized * maxJumpDistance;
    }
}