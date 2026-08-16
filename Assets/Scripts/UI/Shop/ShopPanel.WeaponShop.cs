using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Random = UnityEngine.Random;

// Weapon shop: rarity rolls, shelf of purchasable weapons, level-up.
public partial class ShopPanel
{
    private void SetSpritesToWeaponShop()
    {
        SetWeaponDict();
        SetRarityText();

        const int num_weapons_in_shop = 4;

        _arrayOfChosenRandomWeapons = new GameObject[num_weapons_in_shop];
        for (int i = 0; i < weaponImages.Length; i++)
        {
            // set active if not active
            if (!weaponObjects[i].activeSelf)
            {
                weaponObjects[i].SetActive(true);
            }

            // get rarity
            WeaponTier rarity = CalculateRarity();

            // get random Weapon (fall back to Root if this rarity has no weapons yet)
            if (!_weaponsByRarity.ContainsKey(rarity) || _weaponsByRarity[rarity].Count == 0)
            {
                Debug.LogError($"Keine Waffen für Rarity {rarity} gefunden! Fallback auf Root.");
                rarity = WeaponTier.Root;
            }

            if (_weaponsByRarity.ContainsKey(rarity) && _weaponsByRarity[rarity].Count > 0)
            {
                List<GameObject> matchingWeapons = _weaponsByRarity[rarity];

                int randomIndex = Random.Range(0, matchingWeapons.Count);
                GameObject chosenWeapon = matchingWeapons[randomIndex];

                _arrayOfChosenRandomWeapons[i] = chosenWeapon;
            }
            else
            {
                Debug.LogError("Keine Waffen im Root-Fallback gefunden! Slot wird übersprungen.");
                continue;
            }

            GameObject randomWeapon = _arrayOfChosenRandomWeapons[i];

            WeaponStats weaponStats = randomWeapon.GetComponent<WeaponStats>();
            weaponStats.ApplyStats();
            
            weaponShopBoarderImages[i].sprite = weaponShopBoarderSprites[(int)rarity - 1];

            // get Sprite from Weapon
            Sprite sprite = randomWeapon.GetComponentInChildren<SpriteRenderer>().sprite;

            // set Sprite to Image
            weaponImages[i].sprite = sprite;

            // set Title- and Subtitle-text
            weaponTitles[i].text = weaponStats.weaponName;
            weaponSubtitles[i].text = weaponStats.weaponSubtitle;

            // set information text
            weaponDescriptionText[i].text = weaponStats.GetStatsAsText();

            // set cost to button
            weaponButtonText[i].text = $"Buy - {weaponStats.weaponPrice}";

            // Add Listeners to Button
            weaponButtons[i].onClick.RemoveAllListeners();
            int index = i;
            weaponButtons[i].onClick.AddListener(() => BuyWeapon(index, weaponStats));
            
            rerollCostText.text = "Reroll - " + rerollMechanic.GetRerollPrice();
        }
    }

    private void SetWeaponDict()
    {
        _weaponsByRarity.Clear();
        foreach (var prefab in weaponPrefabs)
        {
            WeaponTier tier = prefab.GetComponent<WeaponStats>().weaponTier;
            if (!_weaponsByRarity.ContainsKey(tier))
            {
                _weaponsByRarity[tier] = new List<GameObject>();
            }
            _weaponsByRarity[tier].Add(prefab);
        }
    }

    private void SetRarityText()
    {
        if (_weaponShopLvl >= weaponShopMaxLvL)
        {
            return;
        }
        var odds = GetCurrentOdds(_weaponShopLvl);

        weaponOddsTexts[0].text = $"{odds.root}%";
        weaponOddsTexts[1].text = $"{odds.shroom}%";
        weaponOddsTexts[2].text = $"{odds.bud}%";
        weaponOddsTexts[3].text = $"{odds.blossom}%";

        if (_weaponShopLvl == 0)
        {
            _currentWeaponShopLvlUpCost = weaponShopBaseLvLUpCost;
        }

        weaponShopLvlUpCostText.text = $"LVL Up - {_currentWeaponShopLvlUpCost}";
        weaponShopLvLText.text = _weaponShopLvl.ToString();
    }

    private WeaponTier CalculateRarity()
    {
        var odds = GetCurrentOdds(_weaponShopLvl);
        RandomRollEvent randomRollEvent = playerTransform.GetComponentInChildren<RandomRollEvent>();

        float roll = randomRollEvent.GetRandomFloatRoll(0f, 100f);


        if (roll <= odds.root)
            return WeaponTier.Root;

        if (roll <= odds.root + odds.shroom)
            return WeaponTier.Shroom;

        if (roll <= odds.root + odds.shroom + odds.bud)
            return WeaponTier.Bud;

        return WeaponTier.Blossom;
    }

    private (float root, float shroom, float bud, float blossom) GetCurrentOdds(float lvl)
    {
        float rawBlossom = ComputeChances(lvl, blossomChanceIncrease, blossomIncreaseAt, Mathf.Infinity, 0f);
        float rawBud     = ComputeChances(lvl, budChanceIncrease, budIncreaseAt, budIncreaseCap, 0f);
        float rawShroom  = ComputeChances(lvl, shroomChanceIncrease, shroomIncreaseAt, shroomIncreaseCap, 0f);

        float blossom = rawBlossom;
        float bud     = Mathf.Max(0, rawBud - blossom);
        float shroom  = Mathf.Max(0, rawShroom - bud - blossom);
        float root    = Mathf.Max(0, _baseChanceForRoot - shroom - bud - blossom);

        return (root, shroom, bud, blossom);
    }

    private float ComputeChances(float playerLvl, float increase, float minLvl, float maxLvl, float baseChance)
    {
        if (playerLvl < minLvl)
        {
            return baseChance;
        }

        float cappedLvl = Mathf.Min(playerLvl, maxLvl);
        float levelDiff = cappedLvl - (minLvl - 1);

        return increase * levelDiff + baseChance;
    }

    private void BuyWeapon(int index, WeaponStats weaponStats)
    {
        float weaponPrice = weaponStats.weaponPrice;
        float playerLightAmount = playerStats.PlayerLightAmount;

        if (playerLightAmount - weaponPrice >= 0)
        {
            GetWeaponAnkers();
            weaponButtons[index].onClick.RemoveAllListeners();
            int inventoryWeaponSlotIndex = GetNextEmptyWeaponSlotIndex();
            int benchIndex = GetNextEmptyBenchSlotIndex();

            Transform targetParent = null;

            if (inventoryWeaponSlotIndex != -1)
            {
                targetParent = inventoryWeaponSlots[inventoryWeaponSlotIndex].Find("WeaponPrefab");
            }
            else if (benchIndex != -1)
            {
                targetParent = weaponShopBenchSlots[benchIndex].transform.Find("WeaponPrefab");
            }
            else
            {
                Debug.Log("Inventar und Bench voll");
                return;
            }

            GameObject chosenWeapon = _arrayOfChosenRandomWeapons[index];
            Instantiate(chosenWeapon, targetParent, false);

            HandlePurchase(weaponPrice);
            weaponObjects[index].SetActive(false);

            RefreshAllUI();
            onWeaponBought?.Invoke();
        }
    }

    private int GetNextEmptyWeaponSlotIndex()
    {
        int index = - 1;
        for (int i = 0; i < inventoryWeaponSlots.Length; i++)
        {
            if (inventoryWeaponSlots[i].Find("WeaponPrefab").childCount == 0)
            {
                index = i;
                break;
            }
        }
        return index;
    }

    private int GetNextEmptyBenchSlotIndex()
    {
        for (int i = 0; i < weaponShopBenchSlots.Length; i++)
        {
            Transform weaponPrefabSlot = weaponShopBenchSlots[i].transform.Find("WeaponPrefab");

            if (weaponPrefabSlot != null && weaponPrefabSlot.childCount == 0)
            {
                return i;
            }
        }
        return -1;
    }

    private void RerollWeapons()
    {
        float rerollPrice = rerollMechanic.GetRerollPrice();
        if (playerStats.PlayerLightAmount >= rerollPrice)
        {
            playerStats.PlayerLightAmount -= rerollPrice;
            rerollMechanic.NumRerolls++;
            float nextRerollCost = rerollMechanic.GetRerollPrice();
            rerollCostText.text = "Reroll - " + nextRerollCost;
            SetSpritesToWeaponShop();
            SetMoneyToUI();
        }
    }

    private void IncreaseWeaponShopLvL()
    {
        if (_weaponShopLvl == (int)weaponShopMaxLvL)
        {
            Debug.Log("Max LVL");
            return;
        }

        if (playerStats.PlayerLightAmount < _currentWeaponShopLvlUpCost)
        {
            Debug.Log("Insufficient funds");
            return;
        }

        HandlePurchase(_currentWeaponShopLvlUpCost);
        _weaponShopLvl++;

        weaponShopLvLText.text = _weaponShopLvl.ToString();

        if (_weaponShopLvl < (int)weaponShopMaxLvL)
        {
            _currentWeaponShopLvlUpCost = Mathf.RoundToInt(_currentWeaponShopLvlUpCost + _currentWeaponShopLvlUpCost * weaponShopLvlUpCostIncrease);
            weaponShopLvlUpCostText.text = $"LVL Up - {_currentWeaponShopLvlUpCost}";
            SetRarityText();
        }
        else
        {
            weaponShopLvlUpCostText.text = "Max LVL";
        }
    }
}
