using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public static class WindowUtility {
    public static void LocationField(ref string field, string label, string windowTitle, string directory, string extension) {
        EditorGUILayout.BeginHorizontal();
        field = EditorGUILayout.TextField(label, field);
        if (GUILayout.Button("Browse", EditorStyles.miniButtonRight)) {
            field = EditorUtility.OpenFilePanel(windowTitle, directory, extension);
        }
        EditorGUILayout.EndHorizontal();
    }

    public static void SaveField(ref string field, string label, string windowTitle, string defaultName, string extension) {
        EditorGUILayout.BeginHorizontal();
        field = EditorGUILayout.TextField(label, field);
        if (GUILayout.Button("Browse", EditorStyles.miniButtonRight)) {
            field = EditorUtility.SaveFilePanelInProject(windowTitle, defaultName, extension, "");
        }
        EditorGUILayout.EndHorizontal();
    }

    public static T GetScriptableObj<T>(string location) where T : ScriptableObject {
        T asset = AssetDatabase.LoadAssetAtPath<T>(location);
        if (asset == null) {
            asset = ScriptableObject.CreateInstance<T>();
            AssetDatabase.CreateAsset(asset, location);
            EditorUtility.SetDirty(asset);
        }

        return asset;
    }
}
