using System.Collections;
using UnityEngine;

public class AnimateSpriteText : MonoBehaviour
{

    public float speed = 0.25f;
    [Button(nameof(s))]
    public bool show;

    public void s()
    {
        Start();
        StartCoroutine(Show());
    }

    private void Start()
    {
        foreach (var sprite in GetComponentsInChildren<SpriteRenderer>())
            sprite.enabled = false;
    }

    public IEnumerator Show()
    {

        foreach (var sprite in GetComponentsInChildren<SpriteRenderer>())
        {
            sprite.enabled = true;
            yield return new WaitForSeconds(speed * Time.deltaTime);
        }

    }

    public IEnumerator Hide()
    {

        foreach (var sprite in GetComponentsInChildren<SpriteRenderer>())
        {
            sprite.enabled = false;
        }

        yield return null;

    }

}