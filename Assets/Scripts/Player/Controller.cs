using UnityEngine;

public class Controller : MonoBehaviour
{

    public void Pause()
    {
        enabled = false;
    }
    
    public void Resume()
    {
        enabled = true;
    }

}
