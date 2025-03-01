using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(LootHandler.LootTable<>.Drop), true)]
public class LootTablePropertyDrawer : PropertyDrawer
{
    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        return EditorGUIUtility.singleLineHeight;
    }

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        var dropProperty = property.FindPropertyRelative("drop");
        var weightProperty = property.FindPropertyRelative("weight");
        var rarityProperty = property.FindPropertyRelative("maxRarity");

        bool isDropItemData = dropProperty.GetUnderlyingType() != typeof(ItemDataBase);
        Rect pos = position;
        pos.width = position.width * 0.5f;
        if (isDropItemData)
            pos.width = position.width * 0.333f;

        Rect p1 = pos, p2 = pos, p3 = pos;
        p2.x += pos.width;
        p3.x += pos.width * 2;

        EditorGUI.PropertyField(p1, dropProperty, GUIContent.none);
        EditorGUI.PropertyField(p2, weightProperty, GUIContent.none);
        if(isDropItemData)
            EditorGUI.PropertyField(p3, rarityProperty, GUIContent.none);
    }
}
