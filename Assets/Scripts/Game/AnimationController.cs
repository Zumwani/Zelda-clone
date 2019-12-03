using System.Collections;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

public abstract class AnimationController<AnimationState> : MonoBehaviour
{

    [Required]
    public SpriteRenderer renderer;

    [Header("Animation Controller")]
    [SerializeField] 
    AnimationState state;
    public float interval = 0.02f;

    [Direction]
    public Vector2Int direction = Vector2Int.up;

    [Header("Properties"), Hidden]
    public string header;

    protected bool isStateLocked;

    public AnimationState State
    {
        get => state;
        set { if (!isStateLocked) state = value; }
    }

    float lastUpdate;
    private void Update()
    {

        if (lastUpdate + interval > Time.time)
            return;
        lastUpdate = Time.time;

        OnUpdate();

    }

    protected abstract void OnUpdate();

    public IEnumerator SetState(AnimationState state, float @for)
    {

        if (isStateLocked)
            yield break;

        var sprite = renderer.sprite;
        this.state = state;
        isStateLocked = true;
        yield return new WaitForSeconds(@for);
        isStateLocked = false;
        State = (AnimationState)System.Enum.ToObject(typeof(AnimationState), 0);
        renderer.sprite = sprite;

    }

}

[System.Serializable]
public class SpritesSet : Set<Sprites>
{ }

[System.Serializable]
public class SpriteSet : Set<Sprite>
{ }

[System.Serializable]
public class Set<T>
{

    public T up;
    public T down;
    public T left;
    public T right;

    public T Direction(Vector2Int dir)
    {

        if (dir.y > 0 && dir.x == 0)
            return up;
        if (dir.y < 0 && dir.x == 0)
            return down;
        if (dir.x < 0)
            return left;
        if (dir.x > 0)
            return right;
        return default;

    }

}

[System.Serializable]
public struct Sprites
{

    public Sprite sprite1;
    public Sprite sprite2;

    public Sprite GetSprite(Sprite current)
    {
        if (current == sprite1)
            return sprite2;
        return sprite1;
    }

}

public class DirectionAttribute : PropertyAttribute
{ }

#if UNITY_EDITOR

[CustomPropertyDrawer(typeof(DirectionAttribute))]
public class DirectionPropertyDrawer : PropertyDrawer
{

    readonly Dictionary<Vector2Int, string> directions = new Dictionary<Vector2Int, string>()
    {
        { Vector2Int.up, "↑" },
        { Vector2Int.down, "↓" },
        { Vector2Int.left, "←" },
        { Vector2Int.right, "→" },
        {Vector2Int.zero, "None" }
    };

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        EditorGUI.LabelField(position, label, new GUIContent(directions[property.vector2IntValue]));
    }

}

#endif