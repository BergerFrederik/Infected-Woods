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
    
    [Header("Particles")]
    [SerializeField] private ParticleSystem burstParticles;
    [SerializeField] private float YAchseesOffset = 1;

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
        SpawnParticles();
        yield return new WaitForSeconds(0.1f);
        spriteRendererEnd.enabled = true;

        Spawn();
    }

    private void Spawn()
    {
        GameObject enemy = Instantiate(_enemyPrefab, transform.position, Quaternion.identity);
        if (enemy.TryGetComponent<EnemyDies>(out EnemyDies enemyDies))
        {
            enemyDies.SetupSpawner(_spawner);
        }

        _spawner.RegisterEnemy(enemy);
        StartCoroutine(DeleteSequence());
    }

    private void SpawnParticles()
    {
        ParticleSystem newBurstParticles = Instantiate(burstParticles, transform.position + new Vector3(0, YAchseesOffset, 0), burstParticles.transform.rotation );
        newBurstParticles.Play();
    }

    private IEnumerator DeleteSequence()
    {
        yield return new WaitForSeconds(deleteTime);
        Destroy(gameObject);
    }
}