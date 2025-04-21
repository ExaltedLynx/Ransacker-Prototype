using System.Collections.Generic;
using UnityEngine;
using static ItemRarity;
using static Enums;
using System;
using UnityEngine.EventSystems;

//TODO switch to instantiating inventory item with the tooltip handler script disabled
public class MerchantItem : MonoBehaviour, IPointerDownHandler
{
    public InventoryItem stockedItem { get; private set; }
    public int sellPrice { get; private set; }
    public ItemType barteredItemType { get; private set; }
    public Rarity rarityCriteria { get; private set; }
    public List<StatType> statsCriteria { get; private set; } = new List<StatType>();
    [SerializeField] private MerchantTooltipHandler tooltipHandler;

    void Start()
    {
        sellPrice = (int)(stockedItem.moneyValue * 1.25f);
    }

    private void Update()
    {
        if(GameManager.Instance.gameIsPaused || GameManager.Instance.shopTutorialEnabled)
            return;

        if(tooltipHandler.hasEnteredItem)
        {
            if (Input.GetKeyDown(KeyCode.LeftShift))
            {
                tooltipHandler.StopAllCoroutines();
                BarterTooltip.Instance.HideTooltip();
                stockedItem.QuickEnableTooltip();
            }
            else if (Input.GetKeyUp(KeyCode.LeftShift))
            {
                ItemTooltip.Instance.HideTooltip();
                tooltipHandler.QuickEnableTooltip();
            }
        }
    }

    private void OnDestroy()
    {
        if(stockedItem != null)
            Destroy(stockedItem.gameObject);
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if(GameManager.Instance.shopTutorialEnabled)
            return;
        
        if (InventoryController.Instance.selectedItem != null)
        {
            bool valid = BarteredItemIsValid(InventoryController.Instance.selectedItem);
            Debug.Log("Is Valid:" + valid);
            if(valid)
            {
                InventoryController.Instance.RemoveHeldItem();
                InsertStockedItem();
            }
        }
        else if(GameManager.Instance.PlayerMoney >= sellPrice)
        {
            GameManager.Instance.SubtractPlayerMoney(sellPrice);
            InsertStockedItem();
        }
    }

    public void SetItem(InventoryItem item)
    {
        stockedItem = item;
        stockedItem.ToggleTooltip();
        //ItemStatsGenerator.AddStatsToItem(item);
    }

    public void AddBarterCriteria()
    {
        barteredItemType = (ItemType) UnityEngine.Random.Range(0, 9);
        rarityCriteria = stockedItem.itemData.itemRarity;

        float statCriteriaChance = 0.25f;
        while(statCriteriaChance > 0f)
        {
            if (UnityEngine.Random.Range(0f, 1f) <= statCriteriaChance)
            {
                statCriteriaChance -= 0.24f;
                StatWeights itemStatWeights = stockedItem.GetStatWeights();
                statsCriteria.Add(itemStatWeights.GetNextAttribute());
            }
            statCriteriaChance -= 0.05f;
        }
    }

    private bool BarteredItemIsValid(InventoryItem item)
    {
        string itemTypeName = Enum.GetName(typeof(ItemType), barteredItemType);
        //Debug.Log("Looking for:" + itemTypeName);
        if (item.itemData is WeaponItem weapon)
        {
            //Debug.Log("Given:" + Enum.GetName(typeof(WeaponItem.WeaponType), weapon.weaponType));
            //Debug.Log(!Enum.GetName(typeof(WeaponItem.WeaponType), weapon.weaponType).Equals(itemTypeName));
            if (!Enum.GetName(typeof(WeaponItem.WeaponType), weapon.weaponType).Equals(itemTypeName))
                return false;
        }
        else if(item.itemData is EquipmentItem equipment)
        {
            //Debug.Log("Given:" + Enum.GetName(typeof(EquipmentItem.EquipmentType), equipment.equipmentType));
            //Debug.Log(!Enum.GetName(typeof(EquipmentItem.EquipmentType), equipment.equipmentType).Equals(itemTypeName));
            if (!Enum.GetName(typeof(EquipmentItem.EquipmentType), equipment.equipmentType).Equals(itemTypeName))
                return false;
        }

        //Debug.Log(item.itemData.itemRarity != rarityCriteria);
        if(item.itemData.itemRarity < rarityCriteria)
            return false;

        if (statsCriteria.Count > 0)
        {
            foreach (StatType statType in statsCriteria)
            {
                foreach (Stat.StatModifier statMod in item.GetItemStats().statList)
                {
                    if (statMod.statType == statType)
                        return true;
                }
            }
        }
        else
            return true;

        return false;
    }

    private void InsertStockedItem()
    {
        if(!InventoryController.Instance.InsertItem(stockedItem, PlayerInventory.Instance))
            LootQueue.Instance.AddLootToQueue(stockedItem);

        BarterTooltip.Instance.HideTooltip();
        ItemTooltip.Instance.HideTooltip();
        stockedItem = null;
    }
}
