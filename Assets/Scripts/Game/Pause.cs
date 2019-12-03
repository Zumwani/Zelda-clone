using System.Collections;
using UnityEngine;

public class Pause : MonoBehaviour
{

    public RectTransform panel;
    public float transitionSpeed;

    public float expandedPos;
    public float collapsedPos;

    bool collapsed;
    public void Toggle()
    {
        if (collapsed)
            Expand();
        else
            Collapse();
    }

    public void Expand()
    {
        StopAllCoroutines();
        StartCoroutine(SetPosition(expandedPos));
        collapsed = false;
    }

    public void Collapse()
    {
        StopAllCoroutines();
        StartCoroutine(SetPosition(collapsedPos));
        collapsed = true;
    }

    IEnumerator SetPosition(float pos)
    {

        float t = 0;
        var prevPos = panel.anchoredPosition.y;
        var newPos = pos;

        while (t <= transitionSpeed)
        {
            panel.anchoredPosition = new Vector2(panel.anchoredPosition.x, Mathf.Lerp(prevPos, newPos, (t / transitionSpeed)));
            t += Time.deltaTime;
            yield return null;
        }

        panel.anchoredPosition = new Vector2(panel.anchoredPosition.x, newPos);

    }

    private void Awake()
    {
        Collapse();
        panel.gameObject.SetActive(true);
    }

    #region Input

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape) || Input.GetKeyDown(KeyCode.Tab))
            Toggle();
    }

    #endregion

}