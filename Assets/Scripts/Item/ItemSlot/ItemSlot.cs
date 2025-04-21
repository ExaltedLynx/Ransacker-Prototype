using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemSlot
{
    protected GameObject equipSlot;
    protected InventoryItem equippedItem;
    public bool isEmpty = true;

    //TODO equip new item first then return the currently equipped one
    public virtual bool setEquippedItem(InventoryItem item)
    {
        bool itemWasInserted = true;
        if (isEmpty == false)
        {
            equippedItem.GetItemStats().statList.ForEach(statMod => Hero.Instance.equippedStatModifiers.Remove(statMod));
            equippedItem.Destroy();
            /*
            //TODO add soulbound tag to differentiate between items that get deleted or returned when swapped
            itemWasInserted = InventoryController.Instance.InsertItem(equippedItem, PlayerInventory.Instance);
            if (itemWasInserted)
            {
                equippedItem.GetItemStats().statList.ForEach(statMod => Hero.Instance.equippedStatModifiers.Remove(statMod));
            }
            else //not enough space for item
                return false;
            */
        }
        else
        {
            isEmpty = false;
        }

        item.gameObject.transform.SetParent(equipSlot.transform);
        RectTransform rectTransform = item.GetComponent<RectTransform>();
        rectTransform.position = equipSlot.transform.position;
        equippedItem = item;

        return itemWasInserted;
    }

    public void UnequipItem()
    {
        bool itemWasInserted = InventoryController.Instance.InsertItem(equippedItem, PlayerInventory.Instance);
        if (itemWasInserted == true)
        {
            equippedItem.GetItemStats().statList.ForEach(statMod => Hero.Instance.equippedStatModifiers.Remove(statMod));
            equippedItem.Destroy();
            isEmpty = true;
            equippedItem = null;
        }
    }

    public InventoryItem getEquippedItem()
    {
        return equippedItem;
    }
}

public class ItemSlot<ItemType> : ItemSlot where ItemType : ItemDataBase
{
    public ItemSlot(GameObject equipSlot) 
    { 
        this.equipSlot = equipSlot;
    }

    public override bool setEquippedItem(InventoryItem item)
    {
       return base.setEquippedItem(item);
    }

    public ItemType getEquippedItemData()
    {
        if(isEmpty)
            return null;

        return (ItemType)equippedItem.itemData;
    }

    public InventoryItem GetEquippedItem()
    {
        return equippedItem;
    }
}