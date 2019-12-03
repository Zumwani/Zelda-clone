using System.Collections;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

[ExecuteAlways]
[RequireComponent(typeof(Image))]
public class Fade : Singleton<Fade>
{

    Image image;

    private void Awake()
    {

        image = GetComponent<Image>();

        if (Application.isPlaying)
            image.color = Color.black;
        StartCoroutine(In(1f));

    }

    public static IEnumerator In(float duration)
    {
        yield return Current.image.LerpColor(duration, Color.clear);
    }

    public static IEnumerator Out(float duration, Color? color = null)
    {
        yield return Current.image.LerpColor(duration, color);
    }

    public async static Task In_Task(float duration)
    {
        await LerpColor(Current.image, duration, Color.clear);
    }

    public async static Task Out_Task(float duration, Color? color = null)
    {
        if (Current)
            await LerpColor(Current.image, duration, color);
    }

    public static async Task LerpColor(Image image, float duration, Color? color = null)
    {

        float t = 0;
        var prevColor = image.color;
        var newColor = color ?? Color.black;

        while (t <= duration)
        {
            if (image)
                image.color = Color.Lerp(prevColor, newColor, (t / duration));
            t += Time.deltaTime;
            await Task.Delay(System.TimeSpan.FromSeconds(Time.deltaTime));
        }

        if (image)
            image.color = newColor;

    }

}
