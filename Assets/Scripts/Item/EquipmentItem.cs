using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "EquipmentItem", menuName = "Item Data/Equipment")]
public class EquipmentItem : ItemDataBase
{
    public enum EquipmentType
    {
        Helmet,
        Chestplate,
        Leggings,
        Boots
    }

    public EquipmentType equipmentType;
}
