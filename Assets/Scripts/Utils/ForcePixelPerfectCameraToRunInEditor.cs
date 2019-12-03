using UnityEngine;
using UnityEngine.U2D;

[ExecuteAlways]
public class ForcePixelPerfectCameraToRunInEditor : MonoBehaviour
{

#if UNITY_EDITOR

    new PixelPerfectCamera camera;
    private void Start()
    {
        camera = GetComponent<PixelPerfectCamera>();
    }

    private void Update()
    {
        camera.runInEditMode = true;
    }

#endif

}
