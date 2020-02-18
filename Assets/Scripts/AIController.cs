using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIController : Controller
{

    public bool isFlying;
    public float speed;
    [Range(0f, 1f)]
    public float turnChance;

    [Required] public OctorockAnimationController animator;
    [Required] public new Rigidbody2D rigidbody;
    [Required] public Collider2D collider;
    [Required] public Camera camera;

    readonly Vector2Int[] directions = new Vector2Int[] 
    { Vector2Int.up, Vector2Int.down, Vector2Int.left, Vector2Int.right };

    Vector2Int direction = Vector2Int.up;

    private void Update()
    {

        if (Random.value > turnChance || IsGoingOffscreen() || !CanGoForward())
            Turn();

        animator.direction = direction;
        GoForward();

        if (direction == Vector2Int.up || direction == Vector2Int.down)
            AlignHorizontally();
        else if (direction == Vector2Int.left || direction == Vector2Int.right)
            AlignVertically();

    }

    void Turn()
    {
        direction = directions.Random();
    }

    (Vector3 origin, Vector3 destination) line;
    bool CanGoForward()
    {
        var origin = transform.position + (0.5f, -0.5f, 0f).ToVector() + (direction.ToVector3() * 0.6f);
        var destination = origin + (direction.ToVector3() * 0.05f);
        line = (origin, destination);
        return !Physics2D.Linecast(origin, destination).collider;
    }

    Vector3 offscreenCheck;
    bool IsGoingOffscreen()
    {
        offscreenCheck = transform.position + direction.ToVector3() + (0.5f, -0.5f, 0f).ToVector();
        return !camera.IsWithinViewport(offscreenCheck);
    }

    Vector2 Forward()
    {
        return (direction.ToVector2() * (speed * Time.deltaTime));
    }

    void GoForward()
    {
        rigidbody.position += Forward();
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawLine(line.origin, line.destination);
        //Gizmos.DrawCube(offscreenCheck, Vector3.one);
    }

    void AlignHorizontally()
    {
        rigidbody.position = new Vector2(LerpAxis(rigidbody.position.x), rigidbody.position.y);
    }

    void AlignVertically()
    {
        rigidbody.position = new Vector2(rigidbody.position.x, LerpAxis(rigidbody.position.y));
    }

    float LerpAxis(float value)
    {
        return Mathf.Lerp(value, Mathf.Round(value), speed * Time.deltaTime);
    }

    public void Pause()
    {
        enabled = false;
    }

    public void Resume()
    {
        enabled = true;
    }

}

[System.Serializable]
public struct Range
{

    public float min;
    public float max;

    public Range(float min, float max)
    {
        this.min = min;
        this.max = max;
    }

    public float Random()
    {
        return UnityEngine.Random.Range(min, max);
    }

}
