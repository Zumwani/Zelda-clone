using System;
using System.Threading.Tasks;
using UnityEngine;

public class WarpTile : Tile
{

    [PickTile(typeof(WarpTile))]
    public Vector2Int m_connectsTo;
    public bool stairsAnimation;

    WarpTile _connectsTo;
    public WarpTile ConnectsTo
    { get {
            if (!_connectsTo && m_connectsTo != Vector2Int.zero)
                _connectsTo = TileManager.Current.GetTile<WarpTile>(m_connectsTo); 
            return _connectsTo;
        } }

    public override void Enter()
    {
        if (enabled)
            DoWarp();
    }

    async void DoWarp()
    {

        if (!ConnectsTo)
            return;

        ConnectsTo.Disable();
        Player.Current.Disable();

        if (stairsAnimation)
            await StairAnimation();
        await Fade.Out_Task(1f);

        Dialogue.Hide();

        Player.Current.Warp(ConnectsTo.position);
        Player.Current.spriteRenderer.sortingLayerName = "Characters";

        await Fade.In_Task(1f);

        Player.Current.Enable();

    }

    async Task StairAnimation()
    {

        Player.Current.transform.position = position.ToVector3Int();
        Player.Current.spriteRenderer.sortingLayerName = "Under map";

        for (int i = 0; i < 10; i++)
        {
            Player.Current.transform.position += Vector3.down / 10;
            await Task.Delay(TimeSpan.FromSeconds(Time.deltaTime));
            //TODO: Step sound
        }

    }

    bool enabled = true;
    public override void Leave()
    {
        enabled = true;
    }

    public void Disable()
    {
        enabled = false;
    }

    public override void DrawGizmos()
    {

        if (!ConnectsTo)
            return;

        Gizmos.color = Color.red;

        var origin = position + (0.5f, 0.5f).ToVector();
        var destination = ConnectsTo.position + (0.5f, 0.5f).ToVector();

        Gizmos.DrawLine(origin, destination);
        DrawRect(Color.red, new Rect(ConnectsTo.position, Vector2.one));

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
#if UNITY_EDITOR
        UnityEditor.Handles.DrawBezier(pos1, pos2, pos1, pos2, color, null, 8);
#endif
    }

}
