using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteAlways]
public class CameraController : Singleton<CameraController>
{

    public new Camera camera;

    public float transitionSpeed;

    private void Awake()
    {
        UpdateNow();
    }

    Vector2Int currentCell;
    private void LateUpdate()
    {

        if (!Player.IsReady)
            return;

        if (currentCell != Map.Current.CurrentCell)
        {
            currentCell = Map.Current.CurrentCell;
            UpdateCamera();
        }

    }

    public void UpdateCamera()
    {
        StartCoroutine(SetPosition(camera, Map.Current.GetCellBoundsFromPlayer().center));
    }

    IEnumerator SetPosition(Camera camera, Vector2 destinationPos)
    {

        float t = 0;
        var prevPos = camera.camera.transform.position;
        var newPos = destinationPos.ToVector3() + camera.offset;

        while (t <= transitionSpeed)
        {
            camera.camera.transform.position = Vector3.Lerp(prevPos, newPos, (t / transitionSpeed));
            t += Time.deltaTime;
            yield return null;
        }

        camera.camera.transform.position = newPos;

    }

    public void UpdateNow()
    {
        StopAllCoroutines();
        camera.camera.transform.position = Map.Current.GetCellBoundsFromPlayer().center.ToVector3() + camera.offset;
    }

    [System.Serializable]
    public struct Camera
    {
        [Required]
        public UnityEngine.Camera camera;
        public Vector3 offset;
    }

}
