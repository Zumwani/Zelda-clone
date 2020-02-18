using UnityEngine;

public class ProjectileAction : AIAction
{

    public enum Mode
    { TargetPlayer, Forward }

    public Mode mode;
    public GameObject prefab;

    protected override void Execute(AIAgent agent)
    {

    }

}
