using UnityEngine;
using UnityEngine.EventSystems;

public class TooltipTrigger : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public void OnPointerEnter(PointerEventData eventData)
    {
        if (transform.Find("WeaponPrefab")?.childCount > 0 || transform.childCount > 0)
        {
            TooltipManager.Instance.Show();
            
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
        TooltipManager.Instance.Hide();
    }
}