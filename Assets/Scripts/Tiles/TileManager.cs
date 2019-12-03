using UnityEngine;
using System.Threading.Tasks;
using System.Linq;

#if UNITY_EDITOR
using UnityEditor;
#endif

[ExecuteAlways]
public class TileManager : Singleton<TileManager>
{

    #region Tiles

    struct Data
    {

#if UNITY_EDITOR

        public static string Path()
        {
            return "Assets/Meta/Resources/Tiles";
        }

        public static string Path(Vector2Int point)
        {
            return Path() + "/" + point.ToString() + ".asset";
        }

        public Tile this[Vector2Int point]
        {
            get => AssetDatabase.LoadMainAssetAtPath(Path(point)) as Tile;
            set
            {
                CleanUp();
                AssetDatabase.DeleteAsset(Path(point));
                AssetDatabase.CreateAsset(value, Path(point));
            }
        }

        public void CleanUp()
        {

            var all = Resources.LoadAll("Tiles/").Where(o => typeof(Tile).IsAssignableFrom(o.GetType())).Cast<Tile>();
            var empty = all.Where(o => o.GetType() == typeof(Tile));
            var unreferenced = empty.Where(t => !all.Any(HasReferencesToOtherTiles)).ToArray();

            foreach (var tile in unreferenced)
                AssetDatabase.DeleteAsset(Path(tile.position));

        }

        static bool HasReferencesToOtherTiles(Tile tile)
        {
            return tile.GetType().GetFields().Any(f => typeof(Tile).IsAssignableFrom(f.FieldType) && f.GetValue(tile) != null);
        }

#else

        public static string Path(Vector2Int point)
        {
            return "Tiles/" + point.ToString();
        }

        public Tile this[Vector2Int point]
        {
            get => Resources.Load<Tile>(Path(point));
        }

#endif

    }

    public void CleanUp()
    {
#if UNITY_EDITOR
        data.CleanUp();
#endif
    }

    Data data;

#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        if (Selection.activeObject is Tile tile)
            tile.DrawGizmos();
    }
#endif

    public Tile this[Vector2Int point, bool create = true]
    {
        get => data[point];
#if UNITY_EDITOR 
        set => data[point] = value;
#endif
    }

    public Tile GetTile(System.Type type, Vector2Int point, bool create = true)
    {
        if (typeof(Tile).IsAssignableFrom(type))
            return (Tile)GetType().GetMethods().FirstOrDefault(m => m.Name == nameof(GetTile) && m.GetParameters().Length == 2).MakeGenericMethod(type).Invoke(this, new object[] { point, create });
        return null;
    }

    public T GetTile<T>(Vector2Int point, bool create = true) where T : Tile
    {

        if (point == Vector2Int.zero)
            return null;

        var tile = data[point];
#if UNITY_EDITOR
        if (tile == null && create)
        {
            tile = ScriptableObject.CreateInstance<T>();
            data[point] = tile;
        }
#endif
        return tile as T;

    }

#if UNITY_EDITOR

    public async Task<Tile> SwapTile(Vector2Int point, System.Type type, bool select = true)
    {

        if (!typeof(Tile).IsAssignableFrom(type))
            return null;

        var tile = data[point];

        if (!tile || (tile && tile.GetType() != type))
        {

            if (select)
            {
                Selection.activeObject = null;
                await Task.Delay(1); //Delay needed in order to give unity time to unselect object
            }

            tile = (Tile)ScriptableObject.CreateInstance(type);
            data[point] = tile;

            if (select)
                Selection.activeObject = tile;
            return tile;

        }
        else
            return null;
    }

#endif

#endregion

#region Ingame

    [HideInInspector] public Vector2Int current;
    Tile currentTile;
    private void Update()
    {

        if (!Application.isPlaying)
            return;

        if (current == Player.Current.Position)
            return;
        current = Player.Current.Position;

        if (currentTile)
            currentTile.Leave();

        var tile = GetTile<Tile>(current, false);
        currentTile = tile;
        if (!tile)
            return;

        tile.Enter();

    }

#endregion

}

