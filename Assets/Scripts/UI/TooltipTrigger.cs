using UnityEngine;
using UnityEngine.EventSystems;

public class TooltipTrigger : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    private GameObject _selectedWeapon;
    
    public void OnPointerClick(PointerEventData eventData)
    {
        if (TooltipManager.Instance.isLocked || transform.Find("WeaponPrefab") == null) 
        {
            return;
        }
        
        if (eventData.button == PointerEventData.InputButton.Right)
        {
            TooltipManager.Instance.HandleInteractionWindow(_selectedWeapon);
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
                _selectedWeapon = wStats.gameObject;
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