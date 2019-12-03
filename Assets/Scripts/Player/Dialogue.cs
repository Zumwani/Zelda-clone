using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class Dialogue : Singleton<Dialogue>
{

    public static IEnumerator Show(string text, float letterDelay = 0.1f)
    {

        Current.text.text = "";

        foreach (var letter in text)
        {
            Current.text.text += letter;
            //TODO: Sound
            yield return new WaitForSeconds(letterDelay);
        }

    }

    public static void Hide()
    {
        Current.text.text = "";
    }

    Text text;

    private void Awake()
    {
        text = GetComponent<Text>();
    }

}
