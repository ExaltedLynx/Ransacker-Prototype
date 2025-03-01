using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Stat;

[Serializable]
public class ItemStats
{
    public List<StatModifier> statList = new List<StatModifier>();

    internal void AddStatModifier(float value, StatType statType, CalcType calcType)
    {
        statList.Add(new StatModifier(value, statType, calcType));
        statList.Sort(SortModifiers);
    }

    internal void AddStatModifier(StatModifier stat)
    {
        statList.Add(stat);
        statList.Sort(SortModifiers);
    }

    //Sorts stats modifiers in the order Flat, Additive, Multiplicative
    public static int SortModifiers(StatModifier stat1, StatModifier stat2)
    {
        return (stat1.calcType + 1) - (stat2.calcType + 1);
    }
}
