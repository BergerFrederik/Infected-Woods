using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

// Item buy/sell transaction flow (non-weapon shop items and the transaction slot UI).
public partial class ShopPanel
{
    public void SelectItemForTransaction(GameObject selectedItem, bool isInShop)
    {
        _itemToTransactOriginalContainer = selectedItem.transform.parent;
        foreach (Transform child in transactionItemContainer)
        {
            Destroy(child.gameObject);
        }

        GameObject displayedItem = Instantiate(selectedItem, transactionItemContainer);

        if (displayedItem.TryGetComponent<ItemSlot>(out var slotScript))
        {
            Destroy(slotScript);
        }

        transactionSectionImage.enabled = true;
        transactionSectionImage.sprite = displayedItem.GetComponent<ItemInformation>().itemIcon;

        String buttonText = isInShop ? "Buy" : "Sell";
        transactionButton.GetComponentInChildren<TextMeshProUGUI>().text = buttonText;
        _isCurrentTransactionBuy = isInShop;
        _itemToTransact = selectedItem.transform;
    }

    private void ItemTransaction()
    {
        if (_isCurrentTransactionBuy) BuyItem(); else SellItem();
    }

    private void BuyItem()
    {
        if (transactionItemContainer.childCount == 0)
        {
            Debug.LogWarning("No item found");
            return;
        }

        if (IsItemInventoryFull())
        {
            Debug.LogWarning("Max item capacity reached");
            return;
        }

        Transform transactionItem = transactionItemContainer.GetChild(0);

        ItemInformation itemInformation = transactionItem.GetComponent<ItemInformation>();
        float itemPrice = itemInformation.itemPrice;
        float playerLightAmount = playerStats.PlayerLightAmount;

        if (playerLightAmount - itemPrice >= 0)
        {
            bool isSpecialItem = itemInformation.tier == ItemInformation.ItemTier.Special;
            bool isBlossomItem = itemInformation.tier == ItemInformation.ItemTier.Blossom;
            if (isSpecialItem || isBlossomItem)
            {
                InstantiateItemForInventory(transactionItem);
                ResetTransactionSection();
            }
            else
            {
                PutItemIntoInventory(transactionItem);
                HandleItemToTransact();
            }
            HandlePurchase(itemPrice);
            HandleUIOnPurchase(itemInformation.tier);

            onItemPurchased?.Invoke();
        }
        else
        {
            Debug.LogWarning("Not enough funds");
        }
    }

    private bool IsItemInventoryFull()
    {
        for (int i = 0; i < itemInventoryContainer.childCount; i++)
        {
            if (itemInventoryContainer.GetChild(i).childCount == 0)
            {
                return false;
            }
        }
        return true;
    }

    public void PutItemIntoInventory(Transform transactionItem)
    {
        if (transactionItem.GetComponent<ItemInformation>().tier == ItemInformation.ItemTier.Special) return;
        for (int i = 0; i < itemInventoryContainer.childCount; i++)
        {
            if (itemInventoryContainer.GetChild(i).childCount == 0)
            {
                Transform itemInventorySlot = itemInventoryContainer.GetChild(i);
                transactionItem.SetParent(itemInventorySlot);
                transactionItem.name = transactionItem.GetComponent<ItemInformation>().itemID;
                Image itemInventorySlotImage = itemInventorySlot.GetComponent<Image>();
                itemInventorySlotImage.enabled = true;
                itemInventorySlotImage.sprite = transactionItem.GetComponent<ItemInformation>().itemIcon;

                InstantiateItemForInventory(transactionItem);
                break;
            }
        }
    }

    private void InstantiateItemForInventory(Transform transactionItem)
    {
        GameObject transactionItemForPlayer = Instantiate(transactionItem.gameObject, playerItemsContainer);
        transactionItemForPlayer.name = transactionItem.GetComponent<ItemInformation>().itemID;

        ItemInformation.ItemTier itemTier = transactionItem.GetComponent<ItemInformation>().tier;
        bool isSpecialItem = itemTier == ItemInformation.ItemTier.Special;
        if (isSpecialItem) return;

        Transform rotatingItem = Instantiate(rotatingItemPrefab, rotatingInventory);
        rotatingItem.GetComponent<Image>().sprite = transactionItem.GetComponent<ItemInformation>().itemIcon;
        rotatingInventory.GetComponent<EllipseRotationUI>().Rebuild();
    }

    private void HandleUIOnPurchase(ItemInformation.ItemTier itemTier)
    {
        bool isSpecialItem = itemTier == ItemInformation.ItemTier.Special;
        bool isBlossomItem = itemTier == ItemInformation.ItemTier.Blossom;

        transactionSectionImage.sprite = null;
        transactionSectionImage.enabled = false;

        if (isSpecialItem || isBlossomItem) return;
        _itemToTransactOriginalContainer.GetComponent<Image>().sprite = itemSoldSprite;
    }

    private void RemoveItemFromInventory(Transform transactionItem)
    {
        Sprite itemIcon = transactionItem.GetComponent<ItemInformation>().itemIcon;
        foreach (Transform child in uiCharacterObject)
        {
            foreach (Transform rotatingItem in child)
            {
                if (rotatingItem.GetComponent<Image>().sprite == itemIcon)
                {
                    Destroy(rotatingItem.gameObject);
                }
            }
        }
    }

    public void InstantTransaction(GameObject itemObj, bool isBuy)
    {
        _itemToTransact = itemObj.transform;
        _itemToTransactOriginalContainer = itemObj.transform.parent;

        if (isBuy)
        {
            InstantBuy(itemObj.GetComponent<ItemInformation>());
        }
        else
        {
            SellItem();
        }
    }

    private void InstantBuy(ItemInformation itemInfo)
    {
        if (IsItemInventoryFull())
        {
            Debug.LogWarning("Max item capacity reached");
            return;
        }

        float playerLightAmount = playerStats.PlayerLightAmount;
        if (playerLightAmount >= itemInfo.itemPrice)
        {
            bool isSpecialItem = itemInfo.tier == ItemInformation.ItemTier.Special;
            bool isBlossomItem = itemInfo.tier == ItemInformation.ItemTier.Blossom;

            if (!isSpecialItem && !isBlossomItem
                && _itemToTransactOriginalContainer != null && _itemToTransactOriginalContainer.GetComponent<Image>() != null)
            {
                _itemToTransactOriginalContainer.GetComponent<Image>().sprite = itemSoldSprite;
            }

            if (isSpecialItem || isBlossomItem)
            {
                InstantiateItemForInventory(_itemToTransact);
            }
            else
            {
                PutItemIntoInventory(_itemToTransact);
            }

            HandlePurchase(itemInfo.itemPrice);
            transactionSectionImage.sprite = null;
            transactionSectionImage.enabled = false;
            onItemPurchased?.Invoke();
        }
        else
        {
            Debug.LogWarning("Not enough funds");
        }
    }

    private void SellItem()
    {
        Transform itemToSell = _itemToTransact;
        if (itemToSell == null && transactionItemContainer.childCount > 0)
        {
            itemToSell = transactionItemContainer.GetChild(0);
        }

        if (itemToSell == null)
        {
            Debug.LogWarning("No item found");
            return;
        }

        ItemInformation itemInfo = itemToSell.GetComponent<ItemInformation>();
        string targetID = itemInfo.itemID;
        bool isBlossom = itemInfo.tier == ItemInformation.ItemTier.Blossom;
        bool isSpecialItem = itemInfo.tier == ItemInformation.ItemTier.Special;

        if (itemToSell.parent != null && itemToSell.parent.GetComponent<Image>() != null)
        {
            Image itemToSellImage = itemToSell.parent.GetComponent<Image>();
            itemToSellImage.sprite = null;
            itemToSellImage.enabled = false;
        }

        float itemRefundValue = itemInfo.itemPrice * (itemSellPercentage / 100f);
        HandlePurchase(-itemRefundValue);

        if (!isBlossom && !isSpecialItem)
        {
            bool foundSlot = false;
            for (int i = 0; i < itemShelfContainer.childCount; i++)
            {
                Transform currentShelf = itemShelfContainer.GetChild(i);

                for (int y = 0; y < currentShelf.childCount; y++)
                {
                    Transform currentItemSlot = currentShelf.GetChild(y);

                    if (currentItemSlot.name == targetID)
                    {
                        currentItemSlot.GetComponent<Image>().sprite = itemInfo.itemIcon;
                        itemToSell.SetParent(currentItemSlot);

                        foundSlot = true;
                        break;
                    }
                }

                if (foundSlot)
                {
                    break;
                }
            }
        }
        else
        {
            OnBlossomItemSold?.Invoke(itemToSell.gameObject);
            Destroy(itemToSell.gameObject);
        }

        for (int i = 0; i < playerItemsContainer.childCount; i++)
        {
            Transform currentPlayerItem = playerItemsContainer.GetChild(i);
            if (currentPlayerItem.name == targetID)
            {
                Destroy(currentPlayerItem.gameObject);
                break;
            }
        }

        onItemSold?.Invoke();
        RemoveItemFromInventory(itemToSell);
        ResetTransactionSection();
        rotatingInventory.GetComponent<EllipseRotationUI>().Rebuild();
    }

    private void HandleItemToTransact()
    {
        Transform transactionItemShelfContainer = _itemToTransact.parent;
        Destroy(_itemToTransact.gameObject);
        transactionItemShelfContainer.GetComponent<Image>().sprite = null;
        _itemToTransact = null;
        transactionButton.GetComponentInChildren<TextMeshProUGUI>().text = "Buy/Sell";
    }

    private void ResetTransactionSection()
    {
        foreach (Transform child in transactionItemContainer)
        {
            Destroy(child.gameObject);
        }

        _itemToTransact = null;
        transactionSectionImage.sprite = null;
        transactionSectionImage.enabled = false;

        transactionButton.GetComponentInChildren<TextMeshProUGUI>().text = "Buy/Sell";
    }
}
