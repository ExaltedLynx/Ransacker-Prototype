using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

public static class EditorUtils
{
    public static void ClearLog()
    {
#if UNITY_EDITOR
        var assembly = Assembly.GetAssembly(typeof(UnityEditor.Editor));
        var type = assembly.GetType("UnityEditor.LogEntries");
        var method = type.GetMethod("Clear");
        method.Invoke(new object(), null);
#endif
    }
}
