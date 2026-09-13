using System.Collections.Generic;
using UnityEngine;

public class EternalTorch : WeaponAbility
{
    [Header("Cone")]
    [SerializeField] private float coneAngle = 60f;
    [SerializeField] private float coneRange = 5f;
    [SerializeField] private ParticleSystem coneParticles;
    [SerializeField] private float particleRotationOffset = 0f;

    private readonly List<Collider2D> enemiesInCone = new List<Collider2D>();

    protected override void Activate()
    {
        Debug.Log("Eternal Torch Activated");
        Vector3 mouseWorldPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mouseWorldPosition.z = transform.position.z;

        Vector2 direction = mouseWorldPosition - transform.position;
        if (direction == Vector2.zero) direction = Vector2.right;
        direction.Normalize();

        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        coneParticles.transform.rotation = Quaternion.Euler(0f, 0f, angle + particleRotationOffset);
        coneParticles.Play();
        FindEnemiesInCone(direction);

        foreach (Collider2D enemyCollider in enemiesInCone)
        {
            ApplyBurn(enemyCollider);
        }
    }

    private void FindEnemiesInCone(Vector2 direction)
    {
        enemiesInCone.Clear();

        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, coneRange);
        foreach (Collider2D collider in colliders)
        {
            if (!collider.CompareTag("Enemy")) continue;

            Vector2 directionToCollider = (Vector2)collider.transform.position - (Vector2)transform.position;
            float angleToCollider = Vector2.Angle(direction, directionToCollider);

            if (angleToCollider <= coneAngle / 2f)
            {
                enemiesInCone.Add(collider);
            }
        }
    }

    private void ApplyBurn(Collider2D enemyCollider)
    {
    }

    private void OnDrawGizmosSelected()
    {
        Vector2 direction = GetGizmoDirection();
        float baseAngle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        Vector3 origin = transform.position;

        Gizmos.color = Color.red;

        Vector3 leftEdge = origin + ConeEdgeOffset(baseAngle - coneAngle / 2f);
        Vector3 rightEdge = origin + ConeEdgeOffset(baseAngle + coneAngle / 2f);
        Gizmos.DrawLine(origin, leftEdge);
        Gizmos.DrawLine(origin, rightEdge);

        const int arcSegments = 20;
        Vector3 previousPoint = leftEdge;
        for (int i = 1; i <= arcSegments; i++)
        {
            float angle = baseAngle - coneAngle / 2f + (coneAngle / arcSegments) * i;
            Vector3 point = origin + ConeEdgeOffset(angle);
            Gizmos.DrawLine(previousPoint, point);
            previousPoint = point;
        }
    }

    private Vector3 ConeEdgeOffset(float angle)
    {
        return Quaternion.Euler(0f, 0f, angle) * Vector2.right * coneRange;
    }

    private Vector2 GetGizmoDirection()
    {
        if (Application.isPlaying && Camera.main != null)
        {
            Vector3 mouseWorldPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            mouseWorldPosition.z = transform.position.z;
            Vector2 direction = mouseWorldPosition - transform.position;
            if (direction != Vector2.zero) return direction.normalized;
        }
        return transform.right;
    }
}
