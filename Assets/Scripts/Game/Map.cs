using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Tilemaps;
#if UNITY_EDITOR
using UnityEditor;
#endif

//TODO: Map overworld, grottos and dungeons
//TODO: Add items
//TODO: Add ui
//TODO: Add enemies
//TODO: Add combat
//TODO: Add warp points
//TODO: Add map
//TODO: Add pause menu
//TODO: Add main menu
//TODO: Add options menu
//TODO: Add music
//TODO: Add difficulty options (enemy health scaling, alternate tiles for bombable (or similar interactables))
//TODO: Add note taking feature

[ExecuteAlways]
public class Map : Singleton<Map>
{

    [Header("Map")]
    public Vector2Int gridSize;
    public Vector2Int cellSize;
    public Tilemaps tilemaps;

    public Dictionary<Vector2Int, Rect> Cells = new Dictionary<Vector2Int, Rect>();
    public Vector2Int CurrentCell { get; private set; }

    public Dictionary<Vector2Int, Cell> customCells = new Dictionary<Vector2Int, Cell>();

    [System.Serializable]
    public class Tilemaps
    {
        public Tilemap overworld;
        public Tilemap grottos;
        public Tilemap dungeons;
        [HideInInspector] public Tilemap current;
    }

    private void Awake()
    {
        GenerateCells();
        OnEnterScreen();
    }

    private void OnValidate()
    {
        GenerateCells();
    }

    private void Update()
    {
        
        //TODO: Change this to check entire cell rather than just pivot point
        var cell = GetCell(Player.Current.transform.position);

        if (cell != CurrentCell)
        {
            OnLeftScreen();
            CurrentCell = cell;
            OnEnterScreen();
        }

    }

    /// <summary>
    /// Returns when active:
    /// Overworld: rect of entire overworld,
    /// Grottos: rect of current grotto,
    /// Underworld: rect of current dungeon
    /// </summary>
    public Rect GetSection()
    {
        //TODO: Add support for underworld
        return new Rect(transform.position.x, transform.position.y, gridSize.x * cellSize.y, gridSize.y * cellSize.y);
    }

    #region Grid

    void GenerateCells()
    {
        Cells.Clear();
        for (int y = 1; y <= gridSize.y; y += 1)
            for (int x = 0; x < gridSize.x; x += 1)
                Cells.Add(new Vector2Int(x, y), new Rect(
                    transform.position.x + (x * cellSize.x), 
                    transform.position.y - (y * cellSize.y), 
                    cellSize.x, cellSize.y));
    }

    public Rect GetCellBoundsFromPlayer()
    {
        return GetCellBoundsFromPoint(Player.Current.transform.position);
    }

    public Rect GetCellBoundsFromPoint(Vector2 point)
    {
        var cell = GetCell(point);
        var pos = (Vector2)transform.position + (cell * (cellSize.x, -cellSize.y).ToVector());
        return new Rect(pos, cellSize);
    }

    public Vector2Int GetCell(Vector2 point)
    {
        return Cells.FirstOrDefault(kvp => kvp.Value.Contains(point)).Key;
    }

    #endregion
    #region Gizmos

    [Header("Guides")]
    public int thickness = 1;
    public Color color;

#if UNITY_EDITOR
    public void OnDrawGizmos()
    {

        if (Cells.Count == 0)
            GenerateCells();

        var x1 = transform.position.x;
        var x2 = transform.position.x + (gridSize.x * cellSize.x);
        for (int y = 0; y >= -gridSize.y; y--)
        {
            var yPos = transform.position.y + (y * cellSize.y);
            DrawLine(color, (x1, yPos).ToVector(), (x2, yPos).ToVector());
        }

        var y1 = transform.position.y;
        var y2 = transform.position.y - (gridSize.y * cellSize.y);
        for (int x = 0; x <= gridSize.x; x++)
        {
            var xPos = transform.position.x + (x * cellSize.x);
            DrawLine(color, (xPos, y1).ToVector(), (xPos, y2).ToVector());
        }

        DrawRect(Color.white, GetCellBoundsFromPlayer());

        foreach (var cell in Cells)
            DrawText(cell.Value.center, cell.Key.x + ", " + cell.Key.y);

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
        Handles.DrawBezier(pos1, pos2, pos1, pos2, color, null, thickness);
    }

    void DrawText(Vector2 pos, string text)
    {
        var content = new GUIContent(text);
        Handles.Label(pos, content, EditorStyles.boldLabel);
    }

#endif

#endregion
    #region Events

    [System.Serializable]
    public struct ScreenEvents
    {
        public UnityEvent enter;
        public UnityEvent left;
    }

    [Header("Events")]
    public ScreenEvents screenEvents;

    public void OnEnterScreen()
    {
        screenEvents.enter.Invoke();
        if (Player.IsReady && customCells.ContainsKey(CurrentCell))
            customCells[CurrentCell].Enter();
    }

    public void OnLeftScreen()
    {
        screenEvents.left.Invoke();
    }

    #endregion

}
