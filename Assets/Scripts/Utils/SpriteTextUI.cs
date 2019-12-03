using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public class SpriteTextUI : SpriteTextBase<Image>
{

    protected override Rect GetBounds(Image renderer)
    {
        return renderer.rectTransform.rect;
    }

    protected override Sprite GetSprite(Image renderer)
    {
        return renderer.sprite;
    }

    protected override void SetSprite(Image renderer, Sprite sprite)
    {
        renderer.sprite = sprite;
    }

    protected override void SetSize(Image renderer)
    {
        renderer.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, fontSize);
        renderer.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, fontSize);
        renderer.transform.localScale = Vector3.one;
    }

}
