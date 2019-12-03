using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Move : Controller
{

    public float speed;
    public float correctionSpeed = 1f;
    
    public Vector2Int Position => transform.position.ToVector2Int();
    public Vector2Int Direction => dir.ToVector2Int();

    static readonly Dictionary<KeyCode, Vector2> directions = new Dictionary<KeyCode, Vector2>()
    {
        { KeyCode.None, Vector2.zero},
        { KeyCode.W,    Vector2.up},
        { KeyCode.S,    Vector2.down},
        { KeyCode.A,    Vector2.left},
        { KeyCode.D,    Vector2.right}
    };

    bool IsMovingHorizontally => keys.Last() == KeyCode.A || keys.LastOrDefault() == KeyCode.D;
    bool IsMovingVertically   => keys.Last() == KeyCode.W || keys.LastOrDefault() == KeyCode.S;

    static readonly List<KeyCode> keys = new List<KeyCode>() { KeyCode.None };

    private void Update()
    {

        dir = GetDirection();

        if (dir != Vector2.zero)
            Player.Current.animator.direction = dir.ToVector2Int();

        Player.Current.animator.NotifyMoving(dir != Vector2Int.zero);

    }

    Vector2 dir;
    private void FixedUpdate()
    {

        if (dir == Vector2.zero)
            return;

        dir = dir.normalized;

        AddSpeed(ref dir);
        AddPosition(ref dir);
        AlignToGrid(ref dir);

        SetPosition(dir);

        //Setting this to zero fixes some issues with random stuttering in movement
        dir = Vector2.zero;

    }

    Vector2 GetDirection()
    {

        //Check if keys are down and add them to list if they are
        foreach (var key in directions.Keys)
            if (Input.GetKeyDown(key))
                keys.Add(key);
            else if (Input.GetKeyUp(key))
                keys.Remove(key);

        if ((keys.Contains(KeyCode.W) && keys.Contains(KeyCode.S)) || 
            (keys.Contains(KeyCode.A) && keys.Contains(KeyCode.D)))

            //Pressing both up/down or left/right should not move character at all
            return Vector2.zero; 

        else 
            //Return the direction of the last key in the list
            return directions[keys.Last()];

    }

    void AddSpeed(ref Vector2 vector)
    {
        vector *= speed * Time.deltaTime;
    }

    void AddPosition(ref Vector2 vector)
    {
        vector += (Vector2)transform.position;
    }

    void AlignToGrid(ref Vector2 vector)
    {

        if (IsMovingHorizontally) //Correct Y axis
            vector = new Vector2(vector.x, LerpAxis(vector.y));

        else if (IsMovingVertically) //Correct X axis
            vector = new Vector2(LerpAxis(vector.x), vector.y);

    }

    float LerpAxis(float value)
    {
        return Mathf.Lerp(value, Mathf.Round(value), speed * Time.deltaTime);
    }

    void SetPosition(Vector2 vector)
    {
        transform.position = vector;
    }

    public void Warp(Vector2Int position)
    {
        var dir = position - (Vector2)transform.position;
        transform.position = position.ToVector3Int();
        Player.Current.animator.direction = (0f, -dir.normalized.y).ToVector().ToVector2Int();
    }

    private void OnDisable()
    {
        keys.Clear();
        keys.Add(KeyCode.None);
    }

    private void OnEnable()
    {
        foreach (var key in directions.Keys)
            if (Input.GetKey(key))
                keys.Add(key);
    }

}
