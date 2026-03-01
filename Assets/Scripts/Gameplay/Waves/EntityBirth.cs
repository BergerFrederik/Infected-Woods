using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class EntityBirth : MonoBehaviour
{
    [SerializeField] private float birthTime = 0.5f;
    [SerializeField] private float targetScaleGrowthFactor = 2f;
    [SerializeField] private float deleteTime = 0.2f;
    [SerializeField] private SpriteRenderer spriteRendererStart;
    [SerializeField] private SpriteRenderer spriteRendererEnd;
    
    private GameObject _enemyPrefab;
    private EntitySpawner _spawner;

    public void Setup(GameObject enemy, EntitySpawner spawner)
    {
        _enemyPrefab = enemy;
        _spawner = spawner;
        StartCoroutine(PrepareSpawn());
    }

    private IEnumerator PrepareSpawn()
    {
        float elapsedTime = 0;
        Vector3 initialScale = transform.localScale;
        Vector3 targetScale = initialScale * targetScaleGrowthFactor;

        while (elapsedTime < birthTime)
        {
            elapsedTime += Time.deltaTime;
            float ratio = elapsedTime / birthTime;
            
            transform.localScale = Vector3.Lerp(initialScale, targetScale, ratio);
            
            yield return null;
        }
        
        spriteRendererStart.enabled = false;
        spriteRendererEnd.enabled = true;
        
        Spawn();
    }

    private void Spawn()
    {
        GameObject enemy = Instantiate(_enemyPrefab, transform.position, Quaternion.identity);
        _spawner.RegisterEnemy(enemy); 
        StartCoroutine(DeleteSequence());
        
    }

    private IEnumerator DeleteSequence()
    {
        yield return new WaitForSeconds(deleteTime);
        Destroy(gameObject);
    }
}