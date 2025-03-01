using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInventory : InventoryGrid
{
    public static PlayerInventory Instance { get; private set; }

    private void Awake()
    {
        Instance = this;
    }

    protected override void Start()
    {
        base.Start();
        ReloadInventory();

    }

    /*
    public override Vector2Int? FindSpaceForItem(InventoryItem itemToInsert)
    {
        int height = (currentGridHeight - 2) - itemToInsert.HEIGHT + 1;
        int width = currentGridWidth - itemToInsert.WIDTH + 1;

        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                if (CheckAvailableSpace(x, y, itemToInsert.WIDTH, itemToInsert.HEIGHT) == true)
                {
                    return new Vector2Int(x, y);
                }
            }
        }
        return null;
    }
    */

    private void ReloadInventory()
    {
        if(DungeonDataCache.Instance.currentFloorNum > 0)
        {
            foreach (CachedItemData cachedItem in PersistInventoryHandler.Instance.inventoryItems)
            {
                InsertItem(Items.InstantiateCachedItem(cachedItem, transform), cachedItem.invPos.x, cachedItem.invPos.y);
            }
        }
    }
}
