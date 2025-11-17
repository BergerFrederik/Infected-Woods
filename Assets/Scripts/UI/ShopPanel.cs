using System;
using UnityEngine;

public class ShopPanel : MonoBehaviour
{
    public static event Action OnShopCycleEnd;
    public void StartNewWave()
    {
        OnShopCycleEnd?.Invoke();
        this.gameObject.SetActive(false);
    }
}
