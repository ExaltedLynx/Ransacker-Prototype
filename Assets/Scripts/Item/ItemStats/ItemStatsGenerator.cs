using static ItemRarity;
using static Stat;
using UnityEngine;
using System;

public class ItemStatsGenerator
{
    private static StatWeights prefStatWeights;

    public static void AddStatsToItem(InventoryItem item)
    {
        ItemDataBase itemData = item.itemData;
        if (itemData is ConsumableItem)
            return;

        int affixNum = 0;
        switch(item.itemData.itemRarity)
        {
            case Rarity.Common: affixNum = 1; break;
            case Rarity.Rare: affixNum = 2; break;
            case Rarity.Epic: affixNum = 3; break;
            case Rarity.Legendary: affixNum = 4; break;
        }

        prefStatWeights = item.GetStatWeights();
        ItemStats itemStats = item.GetItemStats();
        for (int i = 0; i < affixNum; i++)
        {
            StatModifier stat = CreateRandomStat(item.itemData.itemRarity);
            itemStats.AddStatModifier(stat);
        }
    }

    //TODO might need to calculate speed differently, since speed will effect how much player can move in planning phase
    //TODO figure out interesting way to decide between flat, additive, and multiplicative
    private static StatModifier CreateRandomStat(Rarity rarity)
    {
        StatType statType = prefStatWeights.GetNextAttribute();

        float statMult = Mathf.Log10(Hero.Instance.TotalStats);// * DifficultyMultiplier.difficultyScale;
        statMult *= UnityEngine.Random.Range(0.15f, 0.2f);
        //Debug.Log("Mult Base:" + statMultBase + " Stat Mult:" + statMult);

        float value;
        CalcType calcType;
        int rand = UnityEngine.Random.Range(1, 11);
        if (rand <= 6)
        {
            calcType = CalcType.Flat;
            float flatBase = 5f;
            float multVariance = UnityEngine.Random.Range(.9f, 1.3f);
            //Debug.Log("Base: " + (flatBase + (float)rarity) + " Multiplier: " + statMult + " Mult Variance: " + multVariance +  " Diff Scale: " + DifficultyMultiplier.difficultyScale);
            value = Mathf.Pow((flatBase + (float)rarity) * (statMult + multVariance), DifficultyMultiplier.difficultyScale);
            value = (float)Math.Round(value);
        }
        else if (rand <= 9)
        {
            calcType = CalcType.Additive;
            value = statMult;
            value = (float)Math.Round(value, 2);
        }
        else
        {
            calcType = CalcType.Multiplicative;
            value = statMult;
            value = (float)Math.Round(value, 2) / 2;
        }
        //Debug.Log("Type:" + statType + " Value: " + value);
        return new StatModifier(value, statType, calcType);
    }
}
