using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class SpriteText : SpriteTextBase<SpriteRenderer>
{

    protected override Rect GetBounds(SpriteRenderer renderer)
    {
        return new Rect(renderer.bounds.min, renderer.bounds.max);
    }

    protected override Sprite GetSprite(SpriteRenderer renderer)
    {
        return renderer.sprite;
    }

    protected override void SetSprite(SpriteRenderer renderer, Sprite sprite)
    {
        renderer.sprite = sprite;
    }

}
