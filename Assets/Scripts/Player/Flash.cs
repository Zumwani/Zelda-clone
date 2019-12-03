using System.Collections;
using UnityEngine;

public class Flash : MonoBehaviour
{

    [Required]
    public new SpriteRenderer renderer;

    public void Trigger()
    {
        StartCoroutine(DoFlash(2, 0.5f));
    }

    IEnumerator DoFlash(int flashCount, float duration)
    {

        for (int i = 0; i < flashCount; i++)
        {

            Color color = renderer.color;

            yield return renderer.LerpColor(duration / (flashCount * 2), Color.black);
            yield return renderer.LerpColor(duration / (flashCount * 2), color);

        }

    }

}
