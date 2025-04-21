using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemDataBase : ScriptableObject
{
    public Sprite itemIcon;

    [Header("Item Info")]
    //public int ID;
    public string itemName;
    public int baseValue;

    [Header("Size in inventory")]
    public int width = 1;
    public int height = 1;
    
    public ItemRarity.Rarity itemRarity;
}