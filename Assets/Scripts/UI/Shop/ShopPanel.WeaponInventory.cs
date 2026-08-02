using UnityEngine;
using UnityEngine.UI;

// Weapon inventory: slot visuals, bench, combine/move/sell.
public partial class ShopPanel
{
    private void SetSpritesToInventoryOnActivate()
    {
        GetWeaponAnkers();

        for (int i = 0; i < playerWeaponAnkers.Count; i++)
        {
            if (playerWeaponAnkers[i].transform.childCount != 0)
            {
                Transform weaponObj = playerWeaponAnkers[i].transform.GetChild(0);
                weaponObj.SetParent(inventoryWeaponSlots[i].Find("WeaponPrefab"));
            }
        }

        if (characterObject.transform.childCount > 0)
        {
            GameObject chosenCharacterObject = characterObject.transform.GetChild(0).gameObject;
            Transform visuals = chosenCharacterObject.transform.Find("CharacterVisuals");
            if (visuals != null)
                characterInventoryImage.sprite = visuals.GetComponentInChildren<SpriteRenderer>().sprite;
        }
        // RefreshAllUI() below repaints every slot (occupied and empty) from scratch based on the
        // reparenting above, so no per-slot visual state needs to be set in this loop.
        RefreshAllUI();
    }

    private void SetSpritesToInventory()
    {
        for (int i = 0; i < inventoryWeaponSlots.Length; i++)
        {
            Transform backgroundImage = inventoryWeaponSlots[i].Find("BackgroundImage");
            Transform boarderImage = inventoryWeaponSlots[i].Find("BoarderImage");
            Transform weaponImage = inventoryWeaponSlots[i].Find("WeaponImage");
            Transform weaponPrefabSlot = inventoryWeaponSlots[i].Find("WeaponPrefab");

            if (weaponPrefabSlot.childCount != 0)
            {
                Transform weaponObj = weaponPrefabSlot.GetChild(0);
                WeaponStats currentWeaponStats = weaponObj.GetComponent<WeaponStats>();

                Image background = backgroundImage.GetComponent<Image>();
                Image boarder = boarderImage.GetComponent<Image>();
                Image weapon = weaponImage.GetComponent<Image>();

                ApplyWeaponSprite(background, boarder, weapon, currentWeaponStats);

                boarder.color = new Color(boarder.color.r, boarder.color.g, boarder.color.b, 1f);
                weapon.color = new Color(weapon.color.r, weapon.color.g, weapon.color.b, 1f);
            }
            else
            {
                Color cBackground = backgroundImage.GetComponent<Image>().color;
                backgroundImage.GetComponent<Image>().color = new Color(cBackground.r, cBackground.g, cBackground.b, 0f);

                Color cBoarder = boarderImage.GetComponent<Image>().color;
                boarderImage.GetComponent<Image>().color = new Color(cBoarder.r, cBoarder.g, cBoarder.b, 0f);
                boarderImage.GetComponent<Image>().sprite = null;

                Color cWeapon = weaponImage.GetComponent<Image>().color;
                weaponImage.GetComponent<Image>().color = new Color(cWeapon.r, cWeapon.g, cWeapon.b, 0f);
                weaponImage.GetComponent<Image>().sprite = null;
            }
        }
    }

    private void UpdateBenchUI()
    {
        for (int i = 0; i < weaponShopBenchSlots.Length; i++)
        {
            Image backgroundImage = weaponShopBenchSlots[i].Find("BackgroundImage").GetComponent<Image>();
            Image boarderImage = weaponShopBenchSlots[i].Find("BoarderImage").GetComponent<Image>();
            Image weaponImage = weaponShopBenchSlots[i].Find("WeaponImage").GetComponent<Image>();

            Transform weaponPrefabSlot = weaponShopBenchSlots[i].Find("WeaponPrefab");

            if (weaponPrefabSlot.childCount > 0)
            {
                Transform weaponPrefab = weaponPrefabSlot.GetChild(0);
                WeaponStats currentWeaponStats = weaponPrefab.GetComponent<WeaponStats>();

                ApplyWeaponSprite(backgroundImage, boarderImage, weaponImage, currentWeaponStats);
            }
            else
            {
                backgroundImage.color = Color.white;
                backgroundImage.sprite = null;
                weaponImage.sprite = null;
                boarderImage.sprite = null;
            }
        }
    }

    // Shared by SetSpritesToInventory and UpdateBenchUI: background color by level, border by
    // tier, weapon sprite. Alpha/visibility handling differs between the two callers, so that
    // stays with each caller.
    private void ApplyWeaponSprite(Image background, Image border, Image weaponImage, WeaponStats stats)
    {
        background.color = weaponLvLColors[(int)stats.weaponLevel];
        border.sprite = weaponBoarderSprites[(int)stats.weaponTier - 1];
        weaponImage.sprite = stats.GetComponentInChildren<SpriteRenderer>().sprite;
    }

    private void SellWeapon()
    {
        float refund = GetWeaponRefund(selectedWeapon);
        playerStats.PlayerLightAmount += refund;

        selectedWeapon.transform.SetParent(null);

        Destroy(selectedWeapon);
        RefreshAllUI();
    }

    public float GetWeaponRefund(GameObject weapon)
    {
        WeaponStats selectedWeaponStats = weapon.GetComponent<WeaponStats>();
        float weaponPrice = selectedWeaponStats.weaponPrice;
        float weaponLevel = selectedWeaponStats.weaponLevel;

        for (int i = 0; i < weaponLevel; i++) // i < weaponLevel to not double at lvl 0
        {
            weaponPrice *= 2f;
        }

        float refund = weaponPrice * (weaponSellFactor / 100f) + ((bonusRefundPercentagePerLevel / 100f) * weaponPrice * weaponLevel);
        return Mathf.CeilToInt(refund);
    }

    private void CombineWeapon()
    {
        int maxLevel = weaponLvLColors.Length - 1;
        WeaponStats swStats = selectedWeapon.GetComponent<WeaponStats>();


        if (TryCombineInList(inventoryWeaponSlots, swStats, maxLevel)) return;
        TryCombineInList(weaponShopBenchSlots, swStats, maxLevel);
    }


    private bool TryCombineInList(Transform[] slots, WeaponStats swStats, int maxLevel)
    {
        foreach (Transform slot in slots)
        {
            WeaponStats targetStats = slot.GetComponentInChildren<WeaponStats>();

            if (targetStats == null ||
                targetStats.gameObject == selectedWeapon ||
                targetStats.weaponID != swStats.weaponID ||
                targetStats.weaponLevel != swStats.weaponLevel ||
                targetStats.weaponLevel >= maxLevel)
            {
                continue;
            }

            MergeWeapons(selectedWeapon, targetStats);

            CloseInteractionWindow();
            RefreshAllUI();

            return true;
        }
        return false;
    }

    // Levels up targetStats and removes weaponToRemove. Shared by TryCombineInList (combine
    // button on a selected weapon) and MoveWeapon (drag-drop merge).
    private void MergeWeapons(GameObject weaponToRemove, WeaponStats targetStats)
    {
        targetStats.weaponLevel++;
        targetStats.ApplyStats();

        weaponToRemove.transform.SetParent(null);
        weaponToRemove.SetActive(false);
        Destroy(weaponToRemove);
    }


    private void MoveSelectedWeapon()
    {
        bool isBenchSlot = selectedWeapon.transform.GetComponentInParent<UISlotHandler>().isBenchSlot;
        Transform[] targetSlots = isBenchSlot ? inventoryWeaponSlots : weaponShopBenchSlots;

        MoveWeaponToFreeSpot(targetSlots);
        CloseInteractionWindow();
    }

    private void MoveWeaponToFreeSpot(Transform[] targetSlots)
    {
        foreach (Transform slot in targetSlots)
        {
            Transform weaponPrefabContainer = slot.Find("WeaponPrefab");
            bool isSlotOccupied = weaponPrefabContainer.childCount > 0;
            if (isSlotOccupied) continue;

            selectedWeapon.transform.SetParent(weaponPrefabContainer);
            RefreshAllUI();
            return;
        }
    }

    private void CloseInteractionWindow()
    {
        TooltipManager.Instance.UnlockTooltip();
        TooltipManager.Instance.HideInteractionWindow();
        selectedWeapon = null;
    }

    // Drag-and-drop API used by UISlotHandler when a weapon slot is dropped onto another.
    public void MoveWeapon(bool fromBench, int fromIndex, bool toBench, int toIndex)
    {
        GameObject weaponToMove;
        Transform sourceParent;
        Transform targetParent;

        // 1. Quelle bestimmen
        if (fromBench) {
            sourceParent = weaponShopBenchSlots[fromIndex].transform.Find("WeaponPrefab");
        } else {
            sourceParent = inventoryWeaponSlots[fromIndex].Find("WeaponPrefab");
        }

        if (sourceParent == null || sourceParent.childCount == 0) return;
        weaponToMove = sourceParent.GetChild(0).gameObject;

        // 2. Ziel bestimmen
        if (toBench) {
            targetParent = weaponShopBenchSlots[toIndex].transform.Find("WeaponPrefab");
        } else {
            targetParent = inventoryWeaponSlots[toIndex].Find("WeaponPrefab");
        }

        if (targetParent == null) return;

        // 3. Belegungs-Check (Merge oder Swap)
        if (targetParent.childCount > 0)
        {
            GameObject targetWeapon = targetParent.GetChild(0).gameObject;
            WeaponStats statsToMove = weaponToMove.GetComponent<WeaponStats>();
            WeaponStats statsTarget = targetWeapon.GetComponent<WeaponStats>();

            // MERGE LOGIK
            if (statsToMove.weaponID == statsTarget.weaponID && statsToMove.weaponLevel == statsTarget.weaponLevel)
            {
                int maxLevel = weaponLvLColors.Length - 1;

                if (statsTarget.weaponLevel < maxLevel)
                {
                    MergeWeapons(weaponToMove, statsTarget);

                    RefreshAllUI();
                    return;
                }
            }

            // SWAP LOGIK (Wenn kein Merge möglich war oder Max Level erreicht ist)
            // Wir schieben die Ziel-Waffe temporär in den alten Slot der gezogenen Waffe
            targetWeapon.transform.SetParent(sourceParent, false);
            targetWeapon.transform.localPosition = Vector3.zero;

            // Dann schieben wir die gezogene Waffe in den Ziel-Slot
            weaponToMove.transform.SetParent(targetParent, false);
            weaponToMove.transform.localPosition = Vector3.zero;
        }
        else
        {
            // Standard Verschieben (Ziel war leer)
            weaponToMove.transform.SetParent(targetParent, false);
            weaponToMove.transform.localPosition = Vector3.zero;
        }

        RefreshAllUI();
        OnWeaponBought?.Invoke();
    }
}
