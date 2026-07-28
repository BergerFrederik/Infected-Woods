using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EllipseRotationUI : MonoBehaviour
{
    [Header("Ellipse Settings")]
    [SerializeField] private float semiMajorAxis = 5f;
    [SerializeField] private float semiMinorAxis = 3f;
    [SerializeField] private float rotationSpeed = 30f;
    [SerializeField] private Transform inFrontContainer;
    [SerializeField] private Transform inBackContainer;

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
        Rebuild();
        ToggleContainers(true);
    }

    private void OnDisable()
    {
        StopAllCoroutines();
        ToggleContainers(false);
    }
    
    public void Rebuild()
    {
        _centerPosition = transform.position;
        StopAllCoroutines();
        AssignItemsToList();
        ResetPositions();
        AssignItemAngles();
        if (gameObject.activeSelf) StartCoroutine(Rotate());
    }

    private void ResetPositions()
    {
        foreach (Transform item in _itemsToRotate)
        {
            item.SetParent(transform);
        }
    }

    private void AssignItemsToList()
    {
        _itemsToRotate.Clear();
        
        CollectChildrenFrom(transform);
        CollectChildrenFrom(inFrontContainer);
        CollectChildrenFrom(inBackContainer);
    }

    private void CollectChildrenFrom(Transform parent)
    {
        if (parent == null) return;
   
        for (int i = parent.childCount - 1; i >= 0; i--)
        {
            Transform child = parent.GetChild(i);
            if (!_itemsToRotate.Contains(child))
            {
                _itemsToRotate.Add(child);
            }
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
                Transform currentItem = _itemsToRotate[i];
                
                if (currentItem == null) continue;
                if (i >= _itemAngles.Length) break;
                
                
                float itemAngle = (_itemAngles[i] + _globalAngle) % 360f;
                Vector3 localEllipseOffset = GetEllipsePointLocal(itemAngle);
                Transform targetContainer = localEllipseOffset.y > 0f ? inBackContainer : inFrontContainer;
                
                if (currentItem.parent != targetContainer)
                {
                    currentItem.SetParent(targetContainer, true);
                }
                
                currentItem.position = _centerPosition + localEllipseOffset;
            }

            yield return null;
        }
    }

    private void ToggleContainers(bool toggle)
    {
        inFrontContainer.gameObject.SetActive(toggle);
        inBackContainer.gameObject.SetActive(toggle);
    }
}