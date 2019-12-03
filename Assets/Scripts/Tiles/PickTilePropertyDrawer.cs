using System;
using System.Threading;
using UnityEditor;
using UnityEngine;

public class PickTileAttribute : PropertyAttribute
{

    public Type type = typeof(Tile);
    
    public PickTileAttribute()
    { }

    public PickTileAttribute(Type type)
    {
        this.type = type;
    }

}

#if UNITY_EDITOR
[CustomPropertyDrawer(typeof(PickTileAttribute))]
public class PickTilePropertyDrawer : PropertyDrawer<PickTileAttribute>
{

    static CancellationTokenSource token;
    public override async void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {

        property.serializedObject.Update();

        GUI.enabled = false;
        EditorGUI.ObjectField(position, label, TileManager.Current.GetTile<Tile>(property.vector2IntValue), typeof(Tile), true);
        GUI.enabled = true;

        if (Event.current.isKey && Event.current.keyCode == KeyCode.Escape)
        {
            token?.Cancel();
            token = null;
            property.serializedObject.RepaintInspector();
        }
        
        if (GUI.Button(new Rect(position.xMax - 26, position.y, 26, position.height), token != null ? "•" : "☉"))
        {

            if (token != null)
            {
                token.Cancel();
                token = null;
            }
            else
            {

                token = new CancellationTokenSource();
                var (tile, isSuccessful) = await TileTool.PickTile(token.Token);
                token = null;
                if (isSuccessful && tile.position != property.vector2IntValue)
                {

                    if (tile.GetType().Name == typeof(Tile).Name && fieldInfo.FieldType != typeof(Tile))
                        tile = await TileManager.Current.SwapTile(tile.position, fieldInfo.FieldType, false);

                    if (tile?.GetType() == attribute.type)
                    {
                        property.vector2IntValue = tile.position;
                        property.serializedObject.ApplyModifiedProperties();
                    }

                }

            }

        }


    }

}
#endif
