using System.Collections;
using UnityEngine;

public class DeathAnimation : MonoBehaviour
{

    [Required] public new SpriteRenderer renderer;
    [Required] public AIController ai;

    public void Trigger()
    {
        StartCoroutine(DoFlash(2, 0.5f));
    }

    IEnumerator DoFlash(int flashCount, float duration)
    {

        ai.Pause();

        for (int i = 0; i < flashCount; i++)
            yield return renderer.LerpColor(duration / (flashCount * 2), Color.clear);

        Destroy(gameObject);

    }

}