using UnityEngine;

[CreateAssetMenu(fileName = "NewEnemyMovementConfig", menuName = "ScriptableObjects/EnemyMovementConfig")]
public class EnemyMovementConfig : ScriptableObject
{
    public float searchRadius = 1.3f;
    public float maxAvoidanceWeight = 0.85f;
    public float minAvoidanceWeight = 0f;
    public float minSeekDistance = 0.3f;
    public float minFlipInterval = 0.15f;
    public LayerMask enemyLayerMask = ~0;
}
