using System.Collections;
using UnityEngine;

public class DropBehaviour : MonoBehaviour
{
    [Header("Fly Settings")]
    [SerializeField] private float initialFlySpeed;
    [SerializeField] private float speedIncreaseFactor;

    [Header("Lift Settings")]
    [SerializeField] private float liftSpeed;
    [SerializeField] private float liftDistanceY;
    [SerializeField] private float waitTime;

    private bool _isActive;

    private void OnDestroy()
    {
        StopAllCoroutines();
    }

    public void CollectDrop(Transform playerTransform)
    {
        if (!_isActive)
        {
            _isActive = true;
            StartCoroutine(FlyTowardsPlayer(playerTransform));
        }
    }
    private IEnumerator FlyTowardsPlayer(Transform playerTransform)
    {
        //anheben
        float destinationY = transform.position.y + liftDistanceY;
        while (transform.position.y < destinationY)
        {
            transform.position += Vector3.up.normalized * liftSpeed * Time.deltaTime;
            yield return null;
        }
        
        yield return new WaitForSeconds(waitTime);
        
        //fliegen
        float flySpeed = initialFlySpeed;
        while (true)
        {
            Vector3 directionToPlayer = playerTransform.position - this.transform.position;
            transform.position += directionToPlayer.normalized * flySpeed * Time.deltaTime;
            flySpeed += flySpeed * speedIncreaseFactor * Time.deltaTime;
            yield return null;
        }
    }
}
