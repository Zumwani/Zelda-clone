using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Main : MonoBehaviour
{

    public async void OnResume()
    {
        var save = await SaveManager.Load(PlayerPrefs.GetInt("Save.LastSlot", -1));
        SaveManager.Activate(save);
    }

    public void OnQuit() =>
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif

}
