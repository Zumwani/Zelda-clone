using UnityEngine;

public class IsPlayerInFront : AICondition
{

    [Agent]
    OctorockAnimationController animationController;

    public float maxDistance;
    public LayerMask layers;

    protected override bool Evaluate(AIAgent agent)
    {

        var hit = Physics2D.Raycast(transform.position, animationController.direction, maxDistance, layers);
        return (hit.collider.transform == PlayerTracker.Transform);

    }

}
