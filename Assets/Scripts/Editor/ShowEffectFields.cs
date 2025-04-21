#if UNITY_EDITOR
using System.Collections.Generic;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

//[CustomEditor(typeof(ConsumableItem))]
public class ShowEffectFields : Editor
{
    List<SerializedProperty> serializedProperties = new List<SerializedProperty>();
    SerializedObject effectField;
    bool hasEffect = false;

    private void OnEnable()
    {
        var property = serializedObject.FindProperty("effect");
        Debug.Log(property.objectReferenceValue);

        if(property.objectReferenceValue is not null)
        {
            hasEffect = true;
            var effectSerializedObj = new SerializedObject(property.objectReferenceValue);
            effectField = effectSerializedObj;
            effectSerializedObj.Update();
            SerializedProperty objectIterator = effectSerializedObj.GetIterator();
            objectIterator.NextVisible(true);

            while (objectIterator.NextVisible(false))
            {
                Debug.Log(objectIterator.name);
                serializedProperties.Add(objectIterator.Copy());
            }
        }
        //effectSerializedObj.ApplyModifiedProperties();
    }

    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        if(hasEffect)
        {
            EditorGUILayout.PropertyField(serializedProperties[0], new GUIContent("Test"), GUILayout.Height(20));
            effectField.ApplyModifiedProperties();
        }
    }
}
#endif