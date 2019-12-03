#if UNITY_EDITOR
using System;
using System.Linq;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

public static class ScriptableObjectUtility
{

    /// <summary>This makes it easy to create, name and place unique new ScriptableObject asset files.</summary>
    public static T CreateAsset<T>(string path) where T : ScriptableObject
    {

        if (!path.StartsWith("Assets/"))
            path = "Assets/" + path;
        if (!path.EndsWith(".asset"))
            path += ".asset";

        if (AssetDatabase.LoadAssetAtPath<T>(path) is T so)
            return so;

        T asset = ScriptableObject.CreateInstance<T>();
        AssetDatabase.CreateAsset(asset, path);

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        return AssetDatabase.LoadAssetAtPath<T>(path);

    }

    public static void Select(Object asset)
    {
        EditorWindowUtility.ShowInspectorEditorWindow();
        AssetDatabase.OpenAsset(asset);
    }

}

static class EditorWindowUtility
{

    public static void ShowInspectorEditorWindow()
    {
        string inspectorWindowTypeName = "UnityEditor.InspectorWindow";
        ShowEditorWindowWithTypeName(inspectorWindowTypeName);
    }

    public static void ShowSceneEditorWindow()
    {
        string sceneWindowTypeName = "UnityEditor.SceneView";
        ShowEditorWindowWithTypeName(sceneWindowTypeName);
    }

    public static void ShowEditorWindowWithTypeName(string windowTypeName)
    {
        var windowType = typeof(Editor).Assembly.GetType(windowTypeName);
        EditorWindow.GetWindow(windowType);
    }

}
#endif