using System.Linq;
using UnityEngine;

public abstract class SpriteTextBase<Renderer> : MonoBehaviour where Renderer : Component
{

    [TextArea]
    public string text;
    public float fontSize;
    public float spacing;
    public float lineSpacing;
    public bool center;

    float Spacing => spacing == 0 ? 0 : spacing / 10;
    float LineSpacing => lineSpacing == 0 ? 0 : lineSpacing / 10;

    protected abstract Sprite GetSprite(Renderer renderer);
    protected abstract void SetSprite(Renderer renderer, Sprite sprite);
    protected abstract Rect GetBounds(Renderer renderer);
    
    protected virtual void SetSize(Renderer renderer)
    {
        renderer.transform.localScale = Vector2.one * (fontSize / 100);
    }

    protected virtual void SetEnabled(Renderer renderer, bool enabled)
    {
        if (renderer is Behaviour b)
            b.enabled = enabled;
        else if (renderer is UnityEngine.Renderer r)
            r.enabled = enabled;
    }

#if UNITY_EDITOR

    Renderer template;

    protected void OnValidate()
    {

        if (!template)
            template = GetComponent<Renderer>();

        if (spacing <= 0f)
            spacing = 0f;
        if (lineSpacing <= 0f)
            lineSpacing = 0f;

        if (!UnityEditor.EditorApplication.isPlayingOrWillChangePlaymode)
            UnityEditor.EditorApplication.delayCall += UpdateText;

    }

    private void OnDestroy()
    {
    }

    private void OnEnable()
    {
        //UnityEditor.EditorApplication.delayCall -= UpdateText;
    }

    #region Sprites

    Sprite[] sprites;
    Sprite[] Sprites => sprites?.Length > 0 ? sprites : (sprites = Resources.LoadAll<Sprite>("Font"));

    readonly Dictionary<char, string> specialCharacters = new Dictionary<char, string>()
    {
        { '"', "QUOTE" },
        { '?', "QUESTIONMARK" },
        { ' ', "SPACE" }
    };

    Sprite FindSprite(string str)
    {
        str = str.ToUpper();
        return Sprites.FirstOrDefault(s => s.name == str);
    }

    Sprite FindSprite(char str)
    {
        if (specialCharacters.ContainsKey(str))
            return FindSprite(specialCharacters[str]);
        else
            return FindSprite(str.ToString());
    }

    #endregion
    #region Positioning

    void SetPosition(Renderer[][] lines)
    {

        for (int line = 0; line < lines.Length; line++)
            for (int character = 0; character < lines[line].Length; character++)
            {

                var letter = lines[line][character];
                SetEnabled(letter, true);

                SetSize(letter);

                float x, y;
                if (line == 0)
                    y = letter.transform.parent.position.y;
                else
                {

                    var above = lines[line - 1].FirstOrDefault();
                    if (!above)
                        continue; //TODO: Add support for empty lines

                    y = above.transform.TransformPoint((0f, -GetSprite(above).bounds.size.y).ToVector()).y - LineSpacing;

                }

                if (character == 0)
                    x = letter.transform.parent.position.x;
                else
                {

                    var left = lines[line][character - 1];
                    x = left.transform.TransformPoint((GetSprite(left).bounds.size.x, 0f).ToVector()).x + Spacing;

                }

                letter.transform.position = (x, y).ToVector();

            }

        if (!center)
            return;

        var maxWidth = lines.Select(Width).Max();
        if (maxWidth == 0)
            return;

        foreach (var line in lines)
        {
            
            var width = Width(line);
            if (width == maxWidth)
                continue;
            var indent = (maxWidth - width) / 2;

            foreach (var sprite in line)
                sprite.transform.position += Vector3.right * (indent);

        }

    }

    float Width(Renderer[] sprite)
    {

        var x1 = sprite.First().transform.position.x;
        var x2 = sprite.Last().transform.TransformPoint((GetBounds(sprite.Last()).size.x, 0f).ToVector()).x;

        return x2 - x1;

    }

    #endregion

    void UpdateText()
    {

        if (transform)
            transform.ClearChildren();

        UnityEditor.EditorApplication.delayCall -= UpdateText;

        if (text == null)
            return;

        var lines = text.Split(System.Environment.NewLine.ToCharArray()).
            Select(s => s.Trim()).
            Select(s => s.Select(ToSprite).Where(s1 => s1).ToArray()).
            ToArray();

        SetPosition(lines);

    }

    Renderer ToSprite(char str)
    {

        if (FindSprite(str) is Sprite s)
        {

            var obj = new GameObject(str.ToString().ToUpper());
            obj.transform.SetParent(transform);

            return SetSprite(obj, s);

        }

        return default;

    }

    Renderer SetSprite(GameObject obj, Sprite sprite)
    {

        var renderer = obj.AddComponent<Renderer>();

        UnityEditor.EditorUtility.CopySerialized(template, renderer);
        SetSprite(renderer, sprite);

        return renderer;

    }

#endif

}
