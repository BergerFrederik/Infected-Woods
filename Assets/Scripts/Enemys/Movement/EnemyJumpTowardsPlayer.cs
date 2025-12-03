using System.Collections;
using UnityEngine;

public class EnemyJumpTowardsPlayer : MonoBehaviour
{
    [SerializeField] private float maxJumpDistance = 2f;
    [SerializeField] private float jumpDuration = 1f;
    [SerializeField] private float jumpHeight = 3f;
    [SerializeField] private float jumpCooldown = 2f;
    [SerializeField] private Pathfinder pathfinder;
    [SerializeField] private EnemyStats enemyStats;
    [SerializeField] private Collider2D enemyCollider;

    private float restStartTime;

    private Vector2 startPosition;
    private Vector2 endPosition;

    public JumpState currentState = JumpState.Resting;

    public enum JumpState
    {
        Resting,
        Preparing,
        Jumping
    }

    private void Start()
    {
        enemyCollider = GetComponent<Collider2D>();
        restStartTime = Time.time - jumpCooldown;
    }

    private void Update()
    {
        UpdateState();
    }

    private void UpdateState()
    {
        switch (currentState)
        {
            case JumpState.Resting:
                float passedRestTime = Time.time - restStartTime;
                if (passedRestTime >= jumpCooldown)
                {
                    currentState = JumpState.Preparing;
                }
                break;
            case JumpState.Preparing:
                endPosition = CalculateTargetPosition();
                startPosition = transform.position;
                currentState = JumpState.Jumping;
                enemyCollider.enabled = false;
                StartCoroutine(JumpArc(startPosition, endPosition, jumpDuration, jumpHeight));
                break;
            case JumpState.Jumping:
                break;
        }
    }

    private Vector2 CalculateTargetPosition()
    {
        Vector2 playerPosition = pathfinder.GetPlayerPosition();
        Vector2 currentPosition = transform.position;
        Vector2 directionToPlayer = playerPosition - currentPosition;
        float distanceToPlayer = directionToPlayer.magnitude;

        if (distanceToPlayer <= maxJumpDistance)
        {
            return playerPosition;
        }
        else
        {
            return currentPosition + directionToPlayer.normalized * maxJumpDistance;
        }
    }

    private IEnumerator JumpArc(Vector2 start, Vector2 end, float duration, float height)
    {
        pathfinder.enabled = false;
        float timeElapsed = 0f;

        while (timeElapsed < duration)
        {
            float t = timeElapsed / duration;

            // Horizontale Bewegung (Linear)
            Vector2 horizontalMovement = Vector2.Lerp(start, end, t);

            // Vertikale Bewegung (Halbkreisbogen mit Sinus)
            float arc = height * Mathf.Sin(t * Mathf.PI);

            // Setze die neue Position
            transform.position = new Vector3(horizontalMovement.x, horizontalMovement.y + arc, transform.position.z);

            timeElapsed += Time.deltaTime;
            yield return null;
        }

        // Stelle sicher, dass die Landeposition exakt erreicht wird
        transform.position = new Vector3(end.x, end.y, transform.position.z);

        pathfinder.enabled = true;

        // Sprung beendet:
        enemyCollider.enabled = true; // Collider wieder aktivieren
        restStartTime = Time.time; // Ruhephase starten
        currentState = JumpState.Resting; // In den Ruhezustand wechseln
    }
}
