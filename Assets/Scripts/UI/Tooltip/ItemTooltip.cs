using System;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static ItemRarity;
using static Stat;

public class ItemTooltip : TooltipBase
{
    [SerializeField] private TextMeshProUGUI itemName;
    [SerializeField] private TextMeshProUGUI itemStats;
    private string itemStatsText = "";
    private string itemEffectText = "";
    [SerializeField] private TextMeshProUGUI ItemValueText;
    [SerializeField] private DamageTypeIndicator dmgTypeIndicator;

    private const float dmgTypeImageWidth = 100f;
    private float padding = 10f;

    public static ItemTooltip Instance { get; private set; }

    protected override void Awake()
    {
        Instance = this;
        base.Awake();
    }

    protected override void Update()
    {
        base.Update();
        //This is really dumb
        /*
        if (Input.GetKey(KeyCode.LeftShift) && itemEffectText != "")
        {
            itemStats.SetText(itemEffectText);
            float tooltipWidth = Mathf.Max(itemName.renderedWidth, itemStats.renderedWidth);
            if (dmgTypeIndicator.indicatorEnabled)
                tooltipWidth = Mathf.Max(tooltipWidth, itemName.renderedWidth + dmgTypeImageWidth);

            float tooltipHeight = itemName.renderedHeight + itemStats.renderedHeight + ItemValueText.renderedHeight;
            toolTipTransform.sizeDelta = new Vector2(tooltipWidth + padding, tooltipHeight + padding);
        }
        else
        {
            itemStats.SetText(itemStatsText);
            float tooltipWidth = Mathf.Max(itemName.renderedWidth, itemStats.renderedWidth);
            if (dmgTypeIndicator.indicatorEnabled)
                tooltipWidth = Mathf.Max(tooltipWidth, itemName.renderedWidth + dmgTypeImageWidth);

            float tooltipHeight = itemName.renderedHeight + itemStats.renderedHeight + ItemValueText.renderedHeight;
            toolTipTransform.sizeDelta = new Vector2(tooltipWidth + padding, tooltipHeight + padding);
        }
        */
    }

    public override void ShowTooltip()
    {
        gameObject.SetActive(true);
        itemEffectText = "";
        SetText();
        HandleDamageTypeIndicator();
        HandleItemValueText(displayedItem.moneyValue);

        itemName.ForceMeshUpdate();
        itemStats.ForceMeshUpdate();
        ItemValueText.ForceMeshUpdate();

        //weird bug where extra empty lines are randomly added to the tooltip after hovering over certain items
        //FIXED: disabled word wrapping on item value text, weird issue with content size fitter
        float tooltipWidth = Mathf.Max(itemName.renderedWidth, itemStats.renderedWidth);
        if (dmgTypeIndicator.indicatorEnabled)
            tooltipWidth = Mathf.Max(tooltipWidth, itemName.renderedWidth + dmgTypeImageWidth);

        /*
        Debug.Log("Item Name Height: " + itemName.renderedHeight + 
            "\nItem Stats/Desc Height: " + itemStats.renderedHeight + 
            "\nItem Value Height: " + ItemValueText.renderedHeight);
        */
        float tooltipHeight = itemName.renderedHeight + itemStats.renderedHeight + ItemValueText.renderedHeight;
        toolTipTransform.sizeDelta = new Vector2(tooltipWidth + padding, tooltipHeight + padding);
        UpdatePosition();
    }

    protected override void SetText()
    {
        itemName.SetText(displayedItem.itemData.itemName);
        switch(displayedItem.itemData.itemRarity)
        {
            case Rarity.Common:
                itemName.color = Color.white;
                break;
            case Rarity.Rare:
                itemName.color = new Color32(86, 147, 255, 255); //blue
                break;
            case Rarity.Epic:
                itemName.color = new Color32(147, 86, 255, 255); //purple
                break;
            case Rarity.Legendary:
                itemName.color = new Color32(255, 75, 0, 255); //scarlet
                break;
        }

        if (displayedItem.itemData is WeaponItem weapon)
        {
            if (weapon.HasEffect())
                itemEffectText = weapon.GetEffectDesc();

            if(weapon.weaponType != WeaponItem.WeaponType.Shield)
            {
                stringBuilder.AppendLine(" Damage: " + weapon.baseDamage);
                stringBuilder.AppendLine(" Attack Speed: " + weapon.attackSpeed);
            }
        }
        if (displayedItem.itemData is ConsumableItem consumable)
        {
            stringBuilder.AppendLine(consumable.GetEffectDesc());
        }

        foreach (StatModifier mod in displayedItem.GetItemStats().statList)
        {
            string statName = Enum.GetName(typeof(StatType), mod.statType);
            if (mod.calcType == CalcType.Flat) //flat
                stringBuilder.AppendLine(" +" + mod.value + " " + statName);
            else if(mod.calcType == CalcType.Additive) //additive
                stringBuilder.AppendLine(" +" + mod.value * 100 + "% " + statName);
            else //multiplicative
                stringBuilder.AppendLine(" x" + mod.value * 100 + "% " + statName);
        }

        itemStatsText = stringBuilder.ToString();
        itemStats.SetText(itemStatsText);
        stringBuilder.Clear();
    }

    public void SetDisplayedItem(InventoryItem item)
    {
        displayedItem = item;
    }

    private void HandleDamageTypeIndicator()
    {
        if (displayedItem.itemData is WeaponItem weapon)
        {
            dmgTypeIndicator.SetDmgTypeIndicator(weapon.damageType);
        }
        else
        {
            dmgTypeIndicator.DisableDmgTypeIndicator();
        }
    }

    private void HandleItemValueText(int itemValue)
    {
        string formattedValue = itemValue.ToString("N0");
        ItemValueText.SetText(formattedValue);
    }
}
