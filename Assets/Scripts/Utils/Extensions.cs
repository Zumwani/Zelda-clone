using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public static class Extensions
{

    public static Rect LerpTowards(this Rect from, Rect to, float t)
    {
        return new Rect(
            Mathf.Lerp(from.x, to.x, t), 
            Mathf.Lerp(from.y, to.y, t), 
            Mathf.Lerp(from.width, to.width, t), 
            Mathf.Lerp(from.height, to.height, t));
    }

    public static (Vector2 upperLeft, Vector2 upperRight, Vector2 bottomRight, Vector2 bottomLeft) GetCorners(this Rect rect)
    {
        return (rect.UpperLeft(), rect.UpperRight(), rect.BottomRight(), rect.BottomLeft());
    }

    public static Vector2 UpperLeft(this Rect v)
    {
        return new Vector2(v.xMin, v.yMax);
    }

    public static Vector2 UpperRight(this Rect v)
    {
        return new Vector2(v.xMax, v.yMax);
    }

    public static Vector2 BottomRight(this Rect v)
    {
        return new Vector2(v.xMax, v.yMin);
    }

    public static Vector2 BottomLeft(this Rect v)
    {
        return new Vector2(v.xMin, v.yMin);
    }

    public static Vector2 ToVector(this (float x, float y) f)
    {
        return new Vector2(f.x, f.y);
    }

    public static Vector3 ToVector(this (float x, float y, float z) f)
    {
        return new Vector3(f.x, f.y, f.z);
    }

    public static Vector2Int ToVector(this (int x, int y) f)
    {
        return new Vector2Int(f.x, f.y);
    }

    public static Vector3 ToVector3(this Vector2 v, float z = 0f)
    {
        return new Vector3(v.x, v.y, z);
    }

    public static Vector2Int ToVector2Int(this Vector2 v)
    {
        return new Vector2Int(Mathf.RoundToInt(v.x), Mathf.RoundToInt(v.y));
    }

    public static Vector2Int ToVector2Int(this Vector3 vector)
    {
        return new Vector2Int(Mathf.FloorToInt(vector.x), Mathf.FloorToInt(vector.y));
    }

    public static Vector3Int ToVector3Int(this Vector2Int vector, int z = 0)
    {
        return new Vector3Int(vector.x, vector.y, z);
    }

    public static Vector3 ToVector2(this Vector2Int vector)
    {
        return new Vector3(vector.x, vector.y);
    }

    public static Vector3 ToVector3(this Vector2Int vector, float z = 0)
    {
        return new Vector3(vector.x, vector.y, z);
    }

    public static Dictionary<TKey, TValue> ToDictionary<TKey, TValue, T>(this IEnumerable<T> list, System.Func<T, TKey> key, System.Func<T, TValue> value)
    {
        var d = System.Linq.Enumerable.ToDictionary(list, key, value);
        return new Dictionary<TKey, TValue>(d);
    }

    public static void EnsureItemCap<TKey>(this Dictionary<TKey, int> dict, TKey key, int maxCount)
    { 
        if (dict[key] < 0) dict[key] = 0;
        if (dict[key] > maxCount) dict[key] = maxCount;
    }

    public static Rect WithX(this Rect r, float x)
    {
        return new Rect(x, r.y, r.width, r.height);
    }

    public static Rect WithY(this Rect r, float y)
    {
        return new Rect(r.x, y, r.width, r.height);
    }

    public static Rect WithWidth(this Rect r, float width)
    {
        return new Rect(r.x, r.y, width, r.height);
    }

    public static Rect WithHeight(this Rect r, float height)
    {
        return new Rect(r.x, r.y, r.width, height);
    }

    public static IEnumerator LerpColor(this SpriteRenderer image, float duration, Color? color = null)
    {

        float t = 0;
        var prevColor = image.color;
        var newColor = color ?? Color.black;

        while (t <= duration)
        {
            if (image)
                image.color = Color.Lerp(prevColor, newColor, (t / duration));
            t += Time.deltaTime;
            yield return null;
        }

        if (image)
            image.color = newColor;

    }

    public static IEnumerator LerpColor(this Image image, float duration, Color? color = null)
    {

        float t = 0;
        var prevColor = image.color;
        var newColor = color ?? Color.black;

        while (t <= duration)
        {
            if (image)
                image.color = Color.Lerp(prevColor, newColor, (t / duration));
            t += Time.deltaTime;
            yield return null;
        }

        if (image)
            image.color = newColor;

    }

    public static Vector2 StringToVector2(string sVector)
    {
        // Remove the parentheses
        if (sVector.StartsWith("(") && sVector.EndsWith(")"))
            sVector = sVector.Substring(1, sVector.Length - 2);

        // split the items
        string[] sArray = sVector.Split(',');

        // store as a Vector3
        Vector3 result = new Vector3(
            float.Parse(sArray[0]),
            float.Parse(sArray[1]));

        return result;
    }

    public static Vector3 StringToVector3(string sVector)
    {
        // Remove the parentheses
        if (sVector.StartsWith("(") && sVector.EndsWith(")"))
        {
            sVector = sVector.Substring(1, sVector.Length - 2);
        }

        // split the items
        string[] sArray = sVector.Split(',');

        // store as a Vector3
        Vector3 result = new Vector3(
            float.Parse(sArray[0]),
            float.Parse(sArray[1]),
            float.Parse(sArray[2]));

        return result;
    }

    public static void ClearChildren(this Transform transform)
    {
        if (!transform)
            return;
        foreach (var child in transform.GetComponentsInChildren<Transform>().Skip(1))
            if (child && child.gameObject)
                if (Application.isPlaying)
                    Object.Destroy(child.gameObject);
                else
                    Object.DestroyImmediate(child.gameObject);
    }

    public static T Random<T>(this IList<T> list)
    {
        if (list?.Count() == 0) return default;
        return list[UnityEngine.Random.Range(0, list.Count())];
    }

    static readonly Rect one = new Rect(0, 0, 1, 0.8f); //y is offset because of ui
    public static bool IsWithinViewport(this Camera camera, Vector2 point)
    {
        return one.Contains(camera.WorldToViewportPoint(point));
    }

    public static Vector2 ClampToViewport(this Camera camera, Vector2 point)
    {

        var min = camera.ViewportToWorldPoint(one.min);
        var max = camera.ViewportToWorldPoint(one.max);

        float x = point.x;
        float y = point.y;

        if (x < min.x) x = min.x;
        if (x > max.x) x = max.x;
        if (y < min.y) y = min.y;
        if (y > max.y) y = max.y;

        return new Vector2(x, y);

    }

}

/// <summary>A dictionary that behaves reasonably when key doesn't exist.</summary>
public class Dictionary<TKey, TValue> : System.Collections.Generic.Dictionary<TKey, TValue>
{

    public Dictionary()
    { }

    public Dictionary(System.Collections.Generic.Dictionary<TKey, TValue> d)
    {
        foreach (var item in d)
            Add(item.Key, item.Value);
    }

    public new TValue this[TKey key]
    { 
        get { return GetValue(key); }
        set { SetValue(key, value); }
    }

    public new void Add(TKey key, TValue value)
    {
        SetValue(key, value);
    }

    public TValue GetValue(TKey key)
    {
        if (ContainsKey(key))
            return base[key];
        else
            return default;
    }

    public void SetValue(TKey key, TValue value)
    {
        if (ContainsKey(key))
            base[key] = value;
        else
            base.Add(key, value);
    }

}
