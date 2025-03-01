using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PersistInventoryHandler : MonoBehaviour
{
    public static PersistInventoryHandler Instance { get; private set; }

    public List<CachedItemData> inventoryItems { get; private set; } = new();
    public List<CachedItemData> hotbarItems { get; private set; } = new();
    public CachedItemData heldItem { get; private set; } = new();

    public CachedItemData[] playerEquipment { get; private set; } = new CachedItemData[6];

    public void InitInstance()
    {
        Instance = this;
    }

    public void CachePlayerItems()
    {
        CachePlayerInventory(PlayerInventory.Instance);
        CacheHotbar(HotbarInventory.Instance);
        CachePlayerEquipment();
        CachePlayerHeldItem();
    }

    public void ResetInventoryCache()
    {
        inventoryItems = new List<CachedItemData>();
        hotbarItems = new List<CachedItemData>();
        playerEquipment = new CachedItemData[6];
        heldItem = new();
    }


    private void CachePlayerInventory(PlayerInventory inventory)
    {
        //TODO find more efficient way of getting all the items from player's inventory
        for (int x = 0; x < inventory.currentGridWidth; x++)
        {
            for (int y = 0; y < inventory.currentGridHeight; y++)
            {
                InventoryItem item = inventory.inventoryItemSlots[x, y];
                if(item != null)
                {
                    CachedItemData cachedItem = new CachedItemData(item);
                    if (!inventoryItems.Contains(cachedItem))
                        inventoryItems.Add(cachedItem);
                }
            }
        }
    }

    private void CacheHotbar(HotbarInventory inventory)
    {
        //TODO find more efficient way of getting all the items from player's inventory
        for (int x = 0; x < inventory.currentGridWidth; x++)
        {
            for (int y = 0; y < inventory.currentGridHeight; y++)
            {
                InventoryItem item = inventory.inventoryItemSlots[x, y];
                if (item != null)
                {
                    CachedItemData cachedItem = new CachedItemData(item);
                    if (!hotbarItems.Contains(cachedItem))
                        hotbarItems.Add(cachedItem);
                }
            }
        }
    }

    private void CachePlayerEquipment()
    {
        if(Hero.Instance.helmetSlot.GetEquippedItem() != null)
            playerEquipment[0] = new CachedItemData(Hero.Instance.helmetSlot.GetEquippedItem());

        if (Hero.Instance.chestSlot.GetEquippedItem() != null)
            playerEquipment[1] = new CachedItemData(Hero.Instance.chestSlot.GetEquippedItem());

        if (Hero.Instance.pantsSlot.GetEquippedItem() != null)
            playerEquipment[2] = new CachedItemData(Hero.Instance.pantsSlot.GetEquippedItem());

        if (Hero.Instance.bootsSlot.GetEquippedItem() != null)
            playerEquipment[3] = new CachedItemData(Hero.Instance.bootsSlot.GetEquippedItem());

        if (Hero.Instance.mainHand.GetEquippedItem() != null)
            playerEquipment[4] = new CachedItemData(Hero.Instance.mainHand.GetEquippedItem());

        if (Hero.Instance.offHand.GetEquippedItem() != null)
            playerEquipment[5] = new CachedItemData(Hero.Instance.offHand.GetEquippedItem());
    }

    private void CachePlayerHeldItem()
    {
        if(InventoryController.Instance.selectedItem != null)
           heldItem = new CachedItemData(InventoryController.Instance.selectedItem);
    }
}

public struct CachedItemData : IEquatable<CachedItemData>
{
    public ItemDataBase itemData { get; private set; }
    public ItemStats stats { get; private set; }
    public Vector2Int invPos { get; private set; }
    public bool isRotated { get; private set; }

    public CachedItemData(InventoryItem item)
    {
        itemData = item.itemData;
        stats = item.GetItemStats();
        invPos = new Vector2Int(item.gridPositionX, item.gridPositionY);
        isRotated = item.rotated;
    }

    public bool Equals(CachedItemData item)
    {
        if (invPos == item.invPos)
            return true;

        return false;
    }
}
