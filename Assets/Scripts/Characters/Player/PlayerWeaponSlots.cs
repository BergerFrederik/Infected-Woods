using System.Collections.Generic;
using UnityEngine;

public class PlayerWeaponSlots : MonoBehaviour
{
    [SerializeField] private PlayerStats playerStats;
    [SerializeField] private GameManager gameManager;
    [SerializeField] private ShopPanel shopPanel;
    
    [SerializeField] private float playerRadius = 2f;
    
    private void OnEnable()
    {
        playerStats.OnNumWeaponSlotsChanged += SetWeaponSlots;
        gameManager.OnCharacterSet += SortOccupiedSlots;
        shopPanel.OnWeaponBought += SortOccupiedSlots;
    }

    private void OnDisable()
    {
        playerStats.OnNumWeaponSlotsChanged -= SetWeaponSlots;
        gameManager.OnCharacterSet -= SortOccupiedSlots;
        shopPanel.OnWeaponBought -= SortOccupiedSlots;
    }
    
    private void SetWeaponSlots(float numSlots)
    {
        numSlots = Mathf.Max(1f, numSlots);
        float angleBetweenSlots = 360f / numSlots;
        
        for (int i = 0; i < numSlots; i++)
        {
            if (i < this.transform.childCount)
            {
                continue;
            }
            
            GameObject newWeaponSlot = new GameObject($"WeaponAnker{i + 1}");
            newWeaponSlot.transform.SetParent(this.transform);
        }
        
        while (this.transform.childCount > numSlots)
        {
            Destroy(this.transform.GetChild(this.transform.childCount - 1).gameObject);
        }
    }

    private void SortOccupiedSlots()
    {
        List<GameObject> occupiedSlots = new List<GameObject>();

        for (int i = 0; i < this.transform.childCount; i++)
        {
            if (this.transform.GetChild(i).childCount != 0)
            {
                occupiedSlots.Add(this.transform.GetChild(i).gameObject);    
            }
        }
        
        int numOccupiedSlots = occupiedSlots.Count;

        if (numOccupiedSlots == 1)
        {
            occupiedSlots[0].transform.localPosition = new Vector3(0f, 0f, 0f);
            return;
        }
        
        float angleBetweenSlots = 360f / numOccupiedSlots;

        for (int i = 0; i < numOccupiedSlots; i++)
        {
            float currentAngle = angleBetweenSlots * i;
            float angleRad = currentAngle * Mathf.Deg2Rad;
            
            float x = Mathf.Cos(angleRad) * playerRadius;
            float y = Mathf.Sin(angleRad) * playerRadius;
            
            occupiedSlots[i].transform.localPosition = new Vector3(x, y, 0);
        }
    }
}
