using System.Threading.Tasks;
using UnityEngine;

public class IdleAction : AIAction
{

    public float duration;

    protected async override void Execute(AIAgent agent)
    {
        await Task.Delay(Mathf.RoundToInt(duration * 1000));
    }

}
