using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

//[CustomPropertyDrawer(typeof(Effect), true)]
public class EffectPropertyDrawer : PropertyDrawer
{
    EffectSO previousEffectSO;

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        return EditorGUIUtility.standardVerticalSpacing;
    }

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        var effectSOProp = property.serializedObject.FindProperty("effectSO");
        if (effectSOProp != null && effectSOProp.objectReferenceValue != null)
        {
            EffectSO effectSO = effectSOProp.objectReferenceValue as EffectSO;

            if (property.managedReferenceValue == null)
                property.managedReferenceValue = effectSO.GetEffectInstance();

            //EditorGUILayout.BeginFoldoutHeaderGroup(true, property.managedReferenceValue.GetType().Name);
            var propChildren = property.GetEnumerator();
            while (propChildren.MoveNext())
            {
                SerializedProperty child = propChildren.Current as SerializedProperty;
                Debug.Log(child.name + " " + child.editable);
                EditorGUILayout.PropertyField(child, new GUIContent(child.name), GUILayout.Height(20));

            }
            //EditorGUILayout.EndFoldoutHeaderGroup();
        }
        property.serializedObject.ApplyModifiedProperties();
    }
}
