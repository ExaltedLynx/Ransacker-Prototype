using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using static Enums;

public class BarterTooltip : TooltipBase
{
    [SerializeField] private TextMeshProUGUI SellPriceHeaderText;
    [SerializeField] private TextMeshProUGUI SellPriceText;
    [SerializeField] private TextMeshProUGUI headerText;
    [SerializeField] private TextMeshProUGUI itemTypeText;
    [SerializeField] private TextMeshProUGUI rarityText;
    [SerializeField] private TextMeshProUGUI statTypesText;
    private MerchantItem displayedMerchantItem;
    private float padding = 15f;

    public static BarterTooltip Instance { get; private set; }

    protected override void Awake()
    {
        Instance = this;
        base.Awake();
    }

    protected override void Update()
    {
        base.Update();
    }

    public override void ShowTooltip()
    {
        if (displayedItem.itemData is ConsumableItem)
            return;

        gameObject.SetActive(true);
        SetText();
        HandleSellPriceText(displayedMerchantItem.sellPrice);

        float preferredWidth = statTypesText.preferredWidth < itemTypeText.preferredWidth ? itemTypeText.preferredWidth : statTypesText.preferredWidth;
        preferredWidth = preferredWidth < headerText.preferredWidth ? headerText.preferredWidth : preferredWidth;
        preferredWidth = preferredWidth < SellPriceHeaderText.preferredWidth + SellPriceText.preferredWidth ? SellPriceHeaderText.preferredWidth + SellPriceText.preferredWidth : preferredWidth;
        
        toolTipTransform.sizeDelta = new Vector2(preferredWidth + padding, SellPriceHeaderText.preferredHeight + headerText.preferredHeight + (itemTypeText.preferredHeight * 2) + statTypesText.preferredHeight + padding);
        UpdatePosition();
    }

    protected override void SetText()
    {
        itemTypeText.SetText(" Item Type: " + Enum.GetName(typeof(ItemType), displayedMerchantItem.barteredItemType));
        rarityText.SetText(" Rarity: " + Enum.GetName(typeof(ItemRarity.Rarity), displayedMerchantItem.rarityCriteria));
        if (displayedMerchantItem.statsCriteria.Count > 0)
        {
            stringBuilder.Append(" Stats: ");
            foreach (StatType statType in displayedMerchantItem.statsCriteria)
            {
                string statName = Enum.GetName(typeof(StatType), statType);
                stringBuilder.Append(statName);
            }
            statTypesText.SetText(stringBuilder.ToString());
            stringBuilder.Clear();
        }
        else
            statTypesText.SetText("");
    }

    public void SetMerchantItem(MerchantItem merchantItem)
    {
        displayedMerchantItem = merchantItem;
        displayedItem = merchantItem.stockedItem;
    }

    private void HandleSellPriceText(int itemValue)
    {
        string formattedValue = itemValue.ToString("N0");
        SellPriceText.SetText(formattedValue);
    }
}
