using UnityEngine;

public class PlayerTracker : MonoBehaviour
{

    public static Transform Transform { get; private set; }

    private void Start()
    {
        Transform = transform;
    }

}