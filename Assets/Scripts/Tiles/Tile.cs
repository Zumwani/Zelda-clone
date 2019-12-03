using System.Linq;
using UnityEditor;
using UnityEngine;

public class Tile : ScriptableObject
{

    public Vector2Int position => Extensions.StringToVector2(name).ToVector2Int();

    public virtual void Enter() { }
    public virtual void Leave() { }
    public virtual void DrawGizmos() { }
    public virtual void DrawGizmosAlways() { }

}

#if UNITY_EDITOR
[CustomEditor(typeof(Tile), true)]
public class TileEditor : Editor
{

    public override void OnInspectorGUI()
    {

        serializedObject.Update();
        EditorGUI.BeginChangeCheck();

        EditorGUILayout.Space();
        DrawTileSelector();
        EditorGUILayout.Space();
        DrawPropertiesExcluding(serializedObject, "m_Script");
        
        if (EditorGUI.EndChangeCheck())
            serializedObject.ApplyModifiedProperties();

    }

    async void DrawTileSelector()
    {

        var type = serializedObject.targetObject.GetType();

        var point = (Vector2Int)typeof(Tile).GetProperty(nameof(Tile.position)).GetValue(serializedObject.targetObject);
        var types = GetType().Assembly.ExportedTypes.Where(t => typeof(Tile).IsAssignableFrom(t)).ToList();

        var i = types.FindIndex(t => t == type);
        i = EditorGUILayout.Popup(i, types.Select(GetName).ToArray());

        var newType = types.ElementAtOrDefault(i) ?? typeof(Tile);
        if (type != newType)
            await TileManager.Current.SwapTile(point, newType);

    }

    string GetName(System.Type type)
    {
        var name = type.Name.Replace("Tile", "");
        return string.IsNullOrWhiteSpace(name) ? "Empty" : name;
    }

}
#endif
