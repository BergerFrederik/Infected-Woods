using UnityEngine;

public class EntityBirth : MonoBehaviour
{
    private GameObject _enemyPrefab;
    private EntitySpawner _spawner;
    public float birthTime = 0.5f;

    public void Setup(GameObject enemy, EntitySpawner spawner)
    {
        _enemyPrefab = enemy;
        _spawner = spawner;
        Invoke(nameof(Spawn), birthTime);
    }

    private void Spawn()
    {
        GameObject enemy = Instantiate(_enemyPrefab, transform.position, Quaternion.identity);
        _spawner.RegisterEnemy(enemy); // Wichtig für das Limit-System
        Destroy(gameObject);
    }
}