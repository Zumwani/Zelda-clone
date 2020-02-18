using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;

public class CustomAction : AIAction
{

    public enum WaitOption
    { Dont, FixedDuration, Auto }

    [Tooltip(
        "Dont: Proceeds to next action in list next tick\n\n" +
        "FixedDuration: Wait for the specified period of time before continuing to next action in list\n\n" +
        "Auto: If return type is Task then wait, otherwise don't.")]
    public WaitOption wait = WaitOption.Auto;

    [ShowIf(nameof(wait), WaitOption.FixedDuration)]
    public float duration;

    public UnityEvent onTrigger;

    protected async override void Execute(AIAgent agent)
    {

        if (wait == WaitOption.FixedDuration)
        {
            onTrigger.Invoke();
            await Task.Delay(Mathf.RoundToInt(duration * 1000));
        }

        //TODO: Is there a way to wait for tasks?

    }

}
