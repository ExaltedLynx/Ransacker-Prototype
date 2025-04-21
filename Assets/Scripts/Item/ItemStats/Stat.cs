using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class Stat
{
    public float BaseValue { get; init; }
    public float Value { get => currentValue; private set => currentValue = value; }
    [SerializeField] private float currentValue;

    private float totalFlatModifiers = 0;
    private float totalPercentModifiers = 1;
    private float statusFlatModifiers = 0;
    private float statusPercentModifiers = 1;


    public StatType StatType { get; init; }
    public Stat(float baseValue, StatType statType)
    {
        StatType = statType;
        BaseValue = baseValue;
        currentValue = baseValue;
    }

    //Not sure if this math is mathing
    public void AddModifier(StatModifier modifier)
    {
        switch (modifier.calcType)
        {
            case CalcType.Flat:
                totalFlatModifiers += modifier.value;
                break;
            case CalcType.Additive:
                totalPercentModifiers += modifier.value;
                break;
            case CalcType.Multiplicative:
                totalPercentModifiers *= 1 + modifier.value;
                break;
        }
    }

    public void AddStatusModifier(StatModifier modifier)
    {
        switch (modifier.calcType)
        {
            case CalcType.Flat:
                statusFlatModifiers += modifier.value;
                break;
            case CalcType.Additive:
                totalPercentModifiers += modifier.value;
                break;
            case CalcType.Multiplicative:
                statusPercentModifiers *= 1 + modifier.value;
                break;
        }
    }

    public void RemoveStatusModifier(StatModifier modifier)
    {
        switch (modifier.calcType)
        {
            case CalcType.Flat:
                statusFlatModifiers -= modifier.value;
                Value -= modifier.value;
                break;
            case CalcType.Additive:
                statusPercentModifiers -= modifier.value;
                Value /= 1 + modifier.value;
                break;

            case CalcType.Multiplicative:
                statusPercentModifiers /= modifier.value;
                Value /= 1 + modifier.value;
                break;
        }
    }

    public void ApplyModifiers()
    {
        Value = BaseValue;
        Value += totalFlatModifiers;
        Value += statusFlatModifiers;
        Value *= totalPercentModifiers;
        Value *= statusPercentModifiers;
        totalFlatModifiers = 0;
        totalPercentModifiers = 1;
    }

    public void ApplyStatusModifiers()
    {
        Value += statusFlatModifiers;
        Value *= statusPercentModifiers;
    }

    [Serializable]
    public struct StatModifier
    {
        [field: SerializeField]
        public float value { get; private set; }
        [field: SerializeField]
        public StatType statType { get; private set; }
        [field: SerializeField]
        public CalcType calcType { get; private set; }

        public StatModifier(float value, StatType statType, CalcType calcType)
        {
            this.value = value;
            this.statType = statType;
            this.calcType = calcType;
        }
    }
}
public enum CalcType { Flat, Additive, Multiplicative }
public enum StatType { Vitality, Strength, Defense, Dexterity, Intelligence, Speed }

public struct StatWeights
{
    public int totalWeight { get; private set; }
    public int[] weights
    {
        get => _weights;
        init
        {
            _weights = value;
            foreach (int weight in _weights)
                totalWeight += weight;
        }
    }
    private int[] _weights;

    public StatType GetNextAttribute()
    {
        int roll = UnityEngine.Random.Range(0, totalWeight);
        int type = 0;
        for (int i = 0; i < 6; i++)
        {
            roll -= weights[i];
            if (roll < 0)
            {
                RemoveWeight(i); //makes it so that each stat type is unique
                type = i;
                break;
            }
        }
        return (StatType)type;
    }

    private void RemoveWeight(int index)
    {
        totalWeight -= weights[index];
        weights[index] = 0;

    }
}





