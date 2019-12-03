using UnityEngine;

public abstract class Singleton<T> : MonoBehaviour where T : MonoBehaviour
{

    static T current;
    public static T Current { get 
        {
            if (current == null)
                current = FindObjectOfType<T>();
            return current;
        } 
    }

}
