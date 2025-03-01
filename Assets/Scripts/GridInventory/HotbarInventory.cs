using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HotbarInventory : InventoryGrid
{
    public static HotbarInventory Instance { get; private set; }
    private InventoryItem possibleShieldToEquip;

    private void Awake()
    {
        Instance = this;
    }

    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();
        ReloadInventory();
    }

    //Used in editor
    public void UseHotbarItems()
    {
        for (int x = 0; x < currentGridWidth; x++)
        {
            for (int y = 0; y < currentGridHeight; y++)
            {
                InventoryItem hotbarItem = GetItem(x, y);

                if (hotbarItem != null)
                {
                    switch (hotbarItem.itemData)
                    {
                        case ConsumableItem cItem:
                            if(cItem.OnUse())
                                RemoveAndDestroyItem(x, y);
                        break;

                        case EquipmentItem:
                            if (Hero.Instance.EquipItem(hotbarItem))
                            {
                                RemoveItem(x, y);
                                if (hotbarItem.rotated)
                                    hotbarItem.Rotate();
                            }
                        break;

                        case WeaponItem weapon:
                            if(Hero.Instance.mainHand.getEquippedItemData().weaponType != WeaponItem.WeaponType.Sword && weapon.weaponType is WeaponItem.WeaponType.Shield)
                                possibleShieldToEquip = hotbarItem;

                            if (Hero.Instance.EquipItem(hotbarItem))
                            {
                                RemoveItem(x, y);
                                if (hotbarItem.rotated)
                                    hotbarItem.Rotate();

                                //QoL for case when a shield is placed to the left of a sword in the hotbar
                                if(weapon.weaponType is WeaponItem.WeaponType.Sword && possibleShieldToEquip != null)
                                {
                                    Hero.Instance.EquipItem(possibleShieldToEquip);
                                    RemoveItem(possibleShieldToEquip.gridPositionX, possibleShieldToEquip.gridPositionY);
                                    if (possibleShieldToEquip.rotated)
                                        possibleShieldToEquip.Rotate();

                                    possibleShieldToEquip = null;
                                }
                            }
                        break;
                    }
                }
            }
        }
    }

    private void ReloadInventory()
    {
        if (DungeonDataCache.Instance.currentFloorNum > 0)
        {
            foreach (CachedItemData cachedItem in PersistInventoryHandler.Instance.hotbarItems)
            {
                InsertItem(Items.InstantiateCachedItem(cachedItem, transform), cachedItem.invPos.x, cachedItem.invPos.y);
            }
        }
    }
}
