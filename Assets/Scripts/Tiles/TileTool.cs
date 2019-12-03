using UnityEngine;
using System.Threading.Tasks;
using System.Threading;

#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.EditorTools;

[EditorTool("Tile Editor")]
public class TileTool : EditorTool
{

    [SerializeField] Texture2D toolIcon;

    private void OnEnable()
    {
        icon = new GUIContent("Tile Editor", toolIcon, "Tile Editor");
    }

    private void OnDisable()
    {
        if (Selection.activeObject is Tile)
            Selection.activeObject = null;
        TileManager.Current.CleanUp();
    }

    GUIContent icon;
    public override GUIContent toolbarIcon => icon;

    public override void OnToolGUI(EditorWindow window)
    {

        if (Event.current.isKey && Event.current.keyCode == KeyCode.F &&
            Selection.activeObject is Tile tile)
            SceneView.lastActiveSceneView.Frame(new Bounds(tile.position.ToVector3Int(), Vector3.one * 2.5f), false);

        if (Event.current.isKey && Event.current.keyCode == KeyCode.Escape && !IsPickerEnabled)
            Tools.current = Tool.None;

        HandleUtility.AddDefaultControl(GUIUtility.GetControlID(FocusType.Passive));

        Ray worldRay = HandleUtility.GUIPointToWorldRay(Event.current.mousePosition);
        var point = worldRay.origin.ToVector2Int();

        using (new Handles.DrawingScope(Color.black))
        {
            
            if (Selection.activeObject is Tile t)
                DrawRect(Color.red, new Rect(t.position, Vector2.one));

            DrawRect(IsPickerEnabled ? Color.blue : Color.black, new Rect(point, Vector2.one));

        }

        if (Event.current.isMouse && Event.current.type == EventType.MouseDown && Event.current.button == 0)
            SelectTile(point);
        
        SceneView.RepaintAll();

    }

    void SelectTile(Vector2Int point)
    {

        var tile = TileManager.Current.GetTile<Tile>(point);

        if (!IsPickerEnabled)
            ScriptableObjectUtility.Select(tile);
        CurrentTile = tile;

    }

    public static Tile CurrentTile { get; private set; }
    static CancellationToken? picker;
    static bool IsPickerEnabled => picker.HasValue;

    public static async Task<(Tile tile, bool isSuccessful)> PickTile(CancellationToken token)
    {

        if (IsPickerEnabled)
            return (null, false);
        picker = token;

        var t = CurrentTile;
        var tool = Tools.current;
        Tools.current = Tool.Custom;

        while (CurrentTile == t)
        {

            var toolChanged = Tools.current != Tool.Custom;
            var cancelled = token.IsCancellationRequested;

            if (cancelled || toolChanged)
            {
                Reset();
                return (null, false); 
            }
            else
                await Task.Delay(100);

        }
         
        void Reset()
        {
            Tools.current = tool; 
            picker = default;
        }

        Reset();
        return (CurrentTile, true);

    }

    void DrawRect(Color color, Rect rect)
    {
        var (upperLeft, upperRight, bottomRight, bottomLeft) = rect.GetCorners();
        DrawLine(color, upperLeft, upperRight);
        DrawLine(color, upperRight, bottomRight);
        DrawLine(color, bottomRight, bottomLeft);
        DrawLine(color, bottomLeft, upperLeft);
    }

    void DrawLine(Color color, Vector3 pos1, Vector3 pos2)
    {
        Handles.DrawBezier(pos1, pos2, pos1, pos2, color, null, 8);
    }

}

#endif
