using UnityEngine;

public class Player : Singleton<Player>
{

    public static bool IsReady => Current && Current.move;

    [Required] public Move move;
    [Required] public SpriteRenderer spriteRenderer;
    [Required] public LinkAnimationController animator;
    [Required] public Health health;
    public Inventory inventory = new Inventory();
    
    public Vector2Int Position => move.Position;
    public Vector2Int Direction => move.Direction;

    public void Warp(Vector2Int position)
    {
        move.Warp(position);
        CameraController.Current.UpdateNow();
    }

    private void OnEnable() => move.enabled = true;
    private void OnDisable() => move.enabled = false;

    public void Enable()  => enabled = true;
    public void Disable() => enabled = false;

}
