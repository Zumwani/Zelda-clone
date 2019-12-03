using System.Collections;
using UnityEngine;

public class Knockback : MonoBehaviour
{

    public int cells;
    [Required] public Rigidbody2D rigidbody;
    [Required] public Controller controller;
    [Required] public Health health;
    [Required] public Camera camera;

    public void InDirection(Vector2Int direction)
    {
        StartCoroutine(DoKnockback(direction));
    }

    IEnumerator DoKnockback(Vector2Int direction)
    {

        controller.Pause();

        var t = 0f;
        var start = rigidbody.position.ToVector2Int();

        var maxCells = MaxCells(direction);
        if (cells > maxCells)
            cells = maxCells;

        var end = start + (direction * cells);
        while (t <= 0.5f)
        {

            rigidbody.position = Vector2.Lerp(start, end, Mathf.SmoothStep(0, 1, t * 2));

            t += Time.deltaTime;
            yield return null;

        }

        controller.Resume();

    }

    (Vector3 origin, Vector3 destination) line;
    int MaxCells(Vector2Int direction)
    {

        var origin = transform.position + (0.5f, -0.5f, 0f).ToVector() + (direction.ToVector3() * 0.6f);
        var hit = Physics2D.Raycast(origin, direction).point;
        hit = camera.ClampToViewport(hit);
        line = (origin, hit);

        
        return (int)Vector2Int.Distance(transform.position.ToVector2Int(), hit.ToVector2Int()) - 1;

    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawLine(line.origin, line.destination);
    }

}
