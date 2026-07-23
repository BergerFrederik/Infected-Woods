using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EllipseRotation : MonoBehaviour
{
    [Header("Ellipse Settings")]
    [SerializeField] private float semiMajorAxis = 5f;
    [SerializeField] private float semiMinorAxis = 3f;
    [SerializeField] private float rotationSpeed = 30f;

    [Header("Direction")]
    [SerializeField] private FloatDirection direction = FloatDirection.Clockwise;

    [Header("Items")]
    private readonly List<Transform> _itemsToRotate = new();

    public enum FloatDirection
    {
        Clockwise,
        CounterClockwise
    }

    private Vector3 _centerPosition;
    private float[] _itemAngles;
    private float _globalAngle; 

    private void OnEnable()
    {
        if (transform.childCount == 0) return;
        Rebuild();
    }

    private void OnDisable()
    {
        StopAllCoroutines();
    }
    
    public void Rebuild()
    {
        StopAllCoroutines();
        AssignItemsToList();
        _centerPosition = transform.position;
        AssignItemAngles();
        StartCoroutine(Rotate());
    }

    private void AssignItemsToList()
    {
        _itemsToRotate.Clear();
        foreach (Transform item in this.transform)
        {
            _itemsToRotate.Add(item);
        }
    }
    
    private void AssignItemAngles()
    {
        int count = _itemsToRotate.Count;
        _itemAngles = new float[count];

        if (count == 0) return;
        
        for (int i = 0; i < count; i++)
        {
            _itemAngles[i] = (360f / count) * i;
        }
    }

    private Vector3 GetEllipsePointLocal(float angleDeg)
    {
        float rad = angleDeg * Mathf.Deg2Rad;
        float x = Mathf.Cos(rad) * semiMajorAxis;
        float y = Mathf.Sin(rad) * semiMinorAxis;
        return new Vector3(x, y, 0f);
    }

    private IEnumerator Rotate()
    {
        float directionMultiplier = direction == FloatDirection.Clockwise ? -1f : 1f;

        while (true)
        {
            _globalAngle += rotationSpeed * directionMultiplier * Time.deltaTime;
            _globalAngle %= 360f;

            for (int i = 0; i < _itemsToRotate.Count; i++)
            {
                float itemAngle = (_itemAngles[i] + _globalAngle) % 360f;
                _itemsToRotate[i].position = _centerPosition + GetEllipsePointLocal(itemAngle);
            }

            yield return null;
        }
    }
}