using UnityEngine;
using UnityEngine.EventSystems;

public class TooltipTrigger : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    public void OnPointerClick(PointerEventData eventData)
    {
        // Read the weapon from the slot on click - a weapon remembered from hovering may have been
        // moved or sold since, and an empty slot has none at all
        Transform weaponPrefabSlot = transform.Find("WeaponPrefab");
        if (TooltipManager.Instance.isLocked || weaponPrefabSlot == null || weaponPrefabSlot.childCount == 0)
        {
            return;
        }

        if (eventData.button == PointerEventData.InputButton.Right)
        {
            TooltipManager.Instance.HandleInteractionWindow(weaponPrefabSlot.GetChild(0).gameObject);
        }
    }
    public void OnPointerEnter(PointerEventData eventData)
    {
        if (TooltipManager.Instance.isLocked) return;
        
        if (transform.Find("WeaponPrefab")?.childCount > 0 || transform.childCount > 0)
        {
            TooltipManager.Instance.Show();
            TooltipManager.Instance.UnlockTooltip();
            
            WeaponStats wStats = GetComponentInChildren<WeaponStats>();
            ItemInformation iInfo = GetComponentInChildren<ItemInformation>();

            

            if (wStats != null)
            {
                TooltipManager.Instance.SetTooltipData(
                    wStats.weaponName, 
                    wStats.GetStatsAsText(), 
                    wStats.passiveDescription, 
                    wStats.GetComponentInChildren<SpriteRenderer>().sprite
                );
            }
            else if (iInfo != null)
            {
                TooltipManager.Instance.SetTooltipData(
                    iInfo.itemName, 
                    iInfo.GetStatsAsText(), 
                    iInfo.passiveDescription, 
                    iInfo.itemIcon
                );
            }
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (!TooltipManager.Instance.isLocked)
        {
            TooltipManager.Instance.Hide();    
        }
    }
}