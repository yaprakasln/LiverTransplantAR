using UnityEngine;
using UnityEditor;

public class HierarchyLogger : EditorWindow
{
    [MenuItem("Tools/Log Liver Hierarchy")]
    public static void LogHierarchy()
    {
        string path = "Assets/human-liver-and-gallbladder/source/Liver project - Copy/liver exported for sketchfab - ";
        GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(path);
        if (prefab != null) {
            Debug.Log("--- Liver Hierarchy ---");
            LogChildren(prefab.transform, "");
        } else {
            Debug.LogError("Prefab not found at " + path);
        }
    }

    private static void LogChildren(Transform t, string indent)
    {
        Debug.Log(indent + t.name);
        foreach (Transform child in t) {
            LogChildren(child, indent + "  ");
        }
    }
}
