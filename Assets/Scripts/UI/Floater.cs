using UnityEngine;

public class Floater : MonoBehaviour
{
    [SerializeField] private float maxFloatSpeed;
    [SerializeField] private float minFloatSpeed;
    [SerializeField] private float dampingStrength;
    [SerializeField] private float floatHeight;
    [SerializeField] private float offsetRangeY;
    
    [Range(0, 100)]
    [SerializeField] private float dampingStartPercentage;
    private Vector3 _originalPosition;
    private Vector3 _targetPosition;
    private float _lerpTime;

    public bool isSurpessed;

    private enum FloatState
    {
        FloatUp,
        FloatDown
    }

    private FloatState _currentState = FloatState.FloatUp;
    private void OnEnable()
    {
        _originalPosition = transform.position;
        _targetPosition = _originalPosition + Vector3.up * floatHeight;
        
        _lerpTime = Random.Range(0f, offsetRangeY);
        transform.position = Vector3.Lerp(_originalPosition, _targetPosition, _lerpTime);
    }
    
    private void Update()
    {
        if (isSurpessed) return;
        float currentSpeed = maxFloatSpeed;
        float threshold = dampingStartPercentage / 100f;
        
        if (_lerpTime < threshold)
        {
            float startZoneProgress = 1f - (_lerpTime / threshold);
            currentSpeed -= currentSpeed * startZoneProgress * (dampingStrength / 100f);
        }
        else if (_lerpTime > (1f - threshold))
        {
            float endZoneProgress = (_lerpTime - (1f - threshold)) / threshold;
            currentSpeed -= currentSpeed * endZoneProgress * (dampingStrength / 100f);
        }
        currentSpeed = Mathf.Max(currentSpeed, minFloatSpeed);

        _lerpTime += Time.deltaTime * currentSpeed;
        
        ApplyMovement();
    }

    private void ApplyMovement()
    {
        switch (_currentState)
        {
            case FloatState.FloatUp:
                transform.position = Vector3.Lerp(_originalPosition, _targetPosition, _lerpTime);
                if (_lerpTime >= 1f) SwapState(FloatState.FloatDown);
                break;

            case FloatState.FloatDown:
                transform.position = Vector3.Lerp(_targetPosition, _originalPosition, _lerpTime);
                if (_lerpTime >= 1f) SwapState(FloatState.FloatUp);
                break;
        }
    }

    private void SwapState(FloatState newState)
    {
        _lerpTime = 0f;
        _currentState = newState;
    }
}
