using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static WeaponItem;
using static EquipmentItem;
using static ItemRarity;

public class LootHandler
{
    [Serializable]
    public class WeaponLootTable : LootTable<WeaponType>
    {
        public override ItemDataBase TryGetDrop(float dropChance)
        {
            if (UnityEngine.Random.Range(0f, 1f) <= dropChance && dropTable.Count != 0)
            {
                WeaponType weaponType = GetDrop(out Rarity maxDropRarity);
                WeaponItem weapon = Items.FindItem<WeaponItem>(item => item.weaponType == weaponType && item.itemRarity <= maxDropRarity);

                return weapon;
            }
            return null;
        }
    }

    [Serializable]
    public class EquipmentLootTable : LootTable<EquipmentType>
    {
        public override ItemDataBase TryGetDrop(float dropChance)
        {
            if (UnityEngine.Random.Range(0f, 1f) <= dropChance && dropTable.Count != 0)
            {
                EquipmentType equipmentType = GetDrop(out Rarity maxDropRarity);
                EquipmentItem equipment = Items.FindItem<EquipmentItem>(item => item.equipmentType == equipmentType && item.itemRarity <= maxDropRarity);
                return equipment;
            }
            return null;
        }
    }

    [Serializable]
    public class ItemLootTable : LootTable<ItemDataBase>
    {
        public override ItemDataBase TryGetDrop(float dropChance)
        {
            if (UnityEngine.Random.Range(0f, 1f) <= dropChance && dropTable.Count != 0)
            {
                ItemDataBase drop = GetDrop(out _);
                ItemDataBase item = Items.FindItem<ItemDataBase>(item => item.itemName == drop.itemName);
                return item;
            }
            return null;
        }
    }

    public abstract class LootTable<T>
    {
        [Serializable]
        public struct Drop
        {
            public T drop;
            public int weight;
            public Rarity maxRarity;
        }

        [SerializeField] protected List<Drop> dropTable;

        [NonSerialized]
        private int totalWeight = -1;

        public abstract ItemDataBase TryGetDrop(float dropChance);

        public int TotalWeight
        {
            get
            {
                if (totalWeight == -1)
                {
                    CalculateTotalWeight();
                }
                return totalWeight;
            }
        }

        protected T GetDrop(out Rarity maxDropRarity)
        {
            int roll = UnityEngine.Random.Range(0, TotalWeight);
            for (int i = 0; i < dropTable.Count; i++)
            {
                roll -= dropTable[i].weight;
                if (roll < 0)
                {
                    maxDropRarity = dropTable[i].maxRarity;
                    return dropTable[i].drop;
                }
            }
            maxDropRarity = dropTable[0].maxRarity;
            return dropTable[0].drop;
        }

        void CalculateTotalWeight()
        {
            totalWeight = 0;
            for (int i = 0; i < dropTable.Count; i++)
            {
                totalWeight += dropTable[i].weight;
            }
        }
    }
}
