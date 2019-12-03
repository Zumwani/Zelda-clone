using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public class Player : Singleton<Player>
{

    public static bool IsReady => Current && Current.move;

    Move move;
    [HideInInspector] public SpriteRenderer spriteRenderer;
    [HideInInspector] public LinkAnimationController animator;

    private void Awake()
    {
        move = GetComponent<Move>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        animator = GetComponent<LinkAnimationController>();
    }

    public Vector2Int Position
    { get { return move.Position; } }

    public Vector2Int Direction
    { get { return move.Direction; } }

    public void Warp(Vector2Int position)
    {
        move.Warp(position);
        CameraController.Current.UpdateNow();
    }

    private void OnEnable()
    {
        move.enabled = true;
    }

    private void OnDisable()
    {
        move.enabled = false;
    }

    public void Enable()
    { enabled = true; }

    public void Disable()
    { enabled = false; }

}

public enum ProgressKey
{
    None, 
    HasSword,
    BombUpgrade1,
    BombUpgrade2
}

public static class Progress
{

    static readonly Dictionary<ProgressKey, bool> dict = new Dictionary<ProgressKey, bool>();

    public static bool HasCompleted(ProgressKey key)
    {
        return dict[key];
    }

    public static void Complete(ProgressKey key)
    {
        dict[key] = true;
    }

    public static void Uncomplete(ProgressKey key)
    {
        dict[key] = false;
    }

}
