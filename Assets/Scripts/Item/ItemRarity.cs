using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class ItemRarity
{
    public enum Rarity
    {
        Common,
        Rare,
        Epic,
        Legendary
    }

    private static int getRarityWeight(Rarity rarity)
    {
        switch(rarity)
        {
            case Rarity.Common:
                return 6;
            case Rarity.Rare:
                return 3;
            case Rarity.Epic:
                return 2;
            case Rarity.Legendary:
                return 1;
        }
        return 0;
    }

    //lets not recalculate total weight everytime we drop an item
    private static int getTotalWeight()
    {
        int totalWeight = 0;
        for(int i = 0; i < 4; i++)
        {
            totalWeight += getRarityWeight((Rarity)i);
        }
        return totalWeight;
    }

    public static Rarity GetDropRarity()
    {
        float rarityChance = UnityEngine.Random.Range(1, 13);
        float addedRoll = 0;
        for (int i = 0; i < 4; i++)
        {
            addedRoll += getRarityWeight((Rarity)i);
            if (rarityChance < addedRoll)
                return (Rarity)i;
        }
        return Rarity.Common;
    }
}
