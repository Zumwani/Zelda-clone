using UnityEngine;

public class TurnAction : AIAction
{

    [Auto]
    Rigidbody2D rigidbody;

    public enum Mode
    { Random, Clockwise, CounterClockwise }

    public Mode mode;

    public float increment = 90;

    protected override void Execute(AIAgent agent)
    {

        var m = mode;
        if (m == Mode.Random)
            m = Random.value > 0.5 ? Mode.Clockwise : Mode.CounterClockwise;

        if (m == Mode.Clockwise)
            rigidbody.rotation += increment;
        else if (m == Mode.CounterClockwise)
            rigidbody.rotation -= increment;

    }

}
