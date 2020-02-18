using UnityEngine;

public class DialogTile : Tile
{

    public enum TriggerMode
    {
        Once, EveryTime
    }

    [SceneReference]
    public string text;
    public TriggerMode triggerMode;
    public Condition condition;

    public override void Enter()
    {
        if (triggerMode == TriggerMode.EveryTime || (triggerMode == TriggerMode.Once && condition.Eval()))
            if (SceneReference.Find<AnimateSpriteText>(text) is AnimateSpriteText t)
                t.Show();
    }

}
