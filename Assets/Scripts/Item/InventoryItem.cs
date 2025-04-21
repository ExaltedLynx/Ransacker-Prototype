using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using static EquipmentItem;
using static WeaponItem;
using static ItemRarity;

public class InventoryItem : MonoBehaviour
{
    public ItemDataBase itemData;
    private ItemStats stats;
    public int moneyValue { get; private set; }
    public int gridPositionX;
    public int gridPositionY;
    public bool rotated = false;
    [SerializeField] private ItemTooltipHandler tooltipHandler;

    public int HEIGHT
    {
        get
        {
            if(rotated == false)
            {
                return itemData.height;
            }
            return itemData.width;
        }
    }

    public int WIDTH
    {
        get
        {
            if (rotated == false)
            {
                return itemData.width;
            }
            return itemData.height;
        }
    }

    public void Set(ItemDataBase itemData)
    {
        this.itemData = itemData;
        GetComponent<Image>().sprite = itemData.itemIcon;

        Vector2 size = new Vector2();
        size.x = WIDTH * InventoryGrid.tileSizeWidthConst;
        size.y = HEIGHT * InventoryGrid.tileSizeHeightConst;
        GetComponent<RectTransform>().sizeDelta = size;
        moneyValue = itemData.baseValue;
        stats = new ItemStats();
    }

    public void Rotate()
    {
        rotated = !rotated;
        RectTransform rectTransform = GetComponent<RectTransform>();
        rectTransform.rotation = Quaternion.Euler(0, 0, rotated == true ? -90f : 0f);
    }

    public void InitItemStats(ItemStats itemStats = null)
    {
        if (itemStats == null)
            ItemStatsGenerator.AddStatsToItem(this);
        else
            stats = itemStats;

        CalculateItemValue();
    }

    //TODO rebalance to not scale item value as quickly
    private void CalculateItemValue()
    {
        float totalAdditiveStats = 1f;
        float totalMultiplicativeStats = 1f;
        stats.statList.ForEach(stat =>
        {
            switch (stat.calcType)
            {
                case CalcType.Flat:
                    moneyValue += (int)stat.value;
                    break;
                case CalcType.Additive:
                    totalAdditiveStats += (int)stat.value;
                    break;
                case CalcType.Multiplicative:
                    totalMultiplicativeStats += (int)stat.value + 1;
                    break;
            }
            moneyValue = (int)(moneyValue * (totalAdditiveStats + totalMultiplicativeStats));
        });

        float rarityMult = 1;
        switch (itemData.itemRarity)
        {
            case Rarity.Rare:
                rarityMult = 1.5f;
                break;
            case Rarity.Epic:
                rarityMult = 2f;
                break;
            case Rarity.Legendary:
                rarityMult = 3f;
                break;
        }
        moneyValue = (int)(moneyValue * rarityMult);
    }

    public void Destroy()
    {
        Destroy(gameObject);
    }

    public void ToggleTooltip()
    {
        tooltipHandler.HandleToggle();
    }

    public void QuickEnableTooltip()
    {
        tooltipHandler.QuickEnableTooltip();
    }

    public StatWeights GetStatWeights()
    {
        switch(itemData)
        {
            case WeaponItem weapon:
                return wpnAttributePreferences[weapon.weaponType];
            case EquipmentItem equipment:
                return eqpmntAttributePreferences[equipment.equipmentType];
        }
        return new StatWeights();
    }

    //TODO this is really stupid
    public readonly Dictionary<WeaponType, StatWeights> wpnAttributePreferences = new()
    {
        { WeaponType.Sword, new StatWeights { weights = new int[6] {5, 40, 20, 20, 0, 10} } },  //Vit: 5%, Str: 40%, Def: 20%, Dex: 20%, Int 5%, Speed: 10%
        { WeaponType.Dagger, new StatWeights { weights = new int[6] {5, 15, 0, 60, 0, 15} } },  //Vit: 5%, Str: 15%, Dex: 60%, Int 5%, Speed: 15%
        { WeaponType.Axe, new StatWeights { weights = new int[6] {5, 45, 10, 35, 0, 5} } }, //Vit: 5%, Str: 45%, Def 10%, Dex: 35%, Speed: 5%
        { WeaponType.Hammer, new StatWeights { weights = new int[6] {15, 60, 10, 0, 0, 5} } }, //Vit: 15%, Str: 60%, Def 10%, Int 10%, Speed: 5%
        { WeaponType.Shield, new StatWeights { weights = new int[6] {25, 0, 55, 5, 0, 5} } } //Vit: 25%, Def 55%, Dex: 5%, Int 10%, Speed: 5%
    };

    //TODO this is really stupid
    public readonly Dictionary<EquipmentType, StatWeights> eqpmntAttributePreferences = new()
    {
        { EquipmentType.Helmet, new StatWeights { weights = new int[6] {1, 1, 1, 1, 0, 1} } }, //Vit: %, Str: %, Def: %, Dex: %, Int %, Speed: %
        { EquipmentType.Chestplate, new StatWeights { weights = new int[6] {1, 1, 1, 1, 0, 1} } }, //Vit: %, Str: %, Def: %, Dex: %, Int %, Speed: %
        { EquipmentType.Leggings, new StatWeights { weights = new int[6] {1, 1, 1, 1, 0, 1} } }, //Vit: %, Str: %, Def: %, Dex: %, Int %, Speed: %
        { EquipmentType.Boots, new StatWeights { weights = new int[6] {1, 1, 1, 1, 0, 5} } } //Vit: 10%, Str: 10%, Def: 10%, Dex: 10%, Int 10%, Speed: 50%
    };

    public ItemStats GetItemStats() => stats;
}
