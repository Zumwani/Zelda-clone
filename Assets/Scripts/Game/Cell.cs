using System.Collections;
using System.Linq;
using UnityEngine;

public class Cell : MonoBehaviour
{

    [Header("Enter")] 
    public Condition condition;
    public Action action;

    [System.Serializable]
    public struct Action
    {
         
        public GameObject[] objectsToSpawn;

        [TextArea]
        public string dialog;

    }

    public void Enter()
    {
        StartCoroutine(_Enter());
    }

    IEnumerator _Enter()
    {

        Player.Current.Disable();

        var isTrue = condition.Eval();
        foreach (var obj in action.objectsToSpawn)
            if (obj)
                obj.SetActive(isTrue);

        if (isTrue)
            yield return Dialogue.Show(action.dialog);

        Player.Current.Enable();

    }

    void HideEnterAction()
    {

        StopAllCoroutines();
        Dialogue.Hide();
        var renderers = action.objectsToSpawn.Select(o => o.GetComponent<SpriteRenderer>()).Where(r => r).ToArray();
        foreach (var renderer in renderers)
            if (renderer)
                StartCoroutine(renderer.LerpColor(1f, Color.clear));

    }

    private void Awake()
    {
        Map.Current.customCells.Add(Map.Current.GetCell(transform.position), this);
    }

    private void OnValidate()
    {
        Map.Current.customCells.Add(Map.Current.GetCell(transform.position), this);
    }

}

[System.Serializable]
public class Condition
{

    public enum ConditionType
    {
        None, Inventory, Progress
    }

    public ConditionType @if;

    [ShowIf(nameof(@if), ConditionType.Inventory)]
    public Item contains;

    [ShowIf(nameof(@if), ConditionType.Inventory)]
    public int count = 1;

    [ShowIf(nameof(@if), ConditionType.None, invert=true)]
    public bool equals;

    public bool Eval()
    {
        switch (@if)
        {
            case ConditionType.None:
                return true;
            case ConditionType.Inventory:
                return Player.Current.inventory.HasItem(contains) == equals;
            default:
                return true;
        }
    }

}