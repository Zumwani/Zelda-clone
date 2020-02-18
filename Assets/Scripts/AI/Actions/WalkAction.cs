using UnityEngine;

public class WalkAction : AIAction
{

    [Agent] Rigidbody2D rigidbody;
    [Agent] OctorockAnimationController animatorController;

    public enum Target
    { Forward, TowardsPlayer }

    public Target target;

    [Tooltip("Specifies whatever enemy moves on a grid or not.")]
    [ShowIf(nameof(target), Target.TowardsPlayer)]
    public bool isOnGrid;

    public int maxDistance;
    public AnimationCurve path = AnimationCurve.Linear(0, 1f, 1f, 1f);
    public float speed;

    static CanMoveForwardCondition m_condition;
    static CanMoveForwardCondition CanMoveForward => m_condition ? m_condition : (m_condition = CreateInstance<CanMoveForwardCondition>());

    protected async override void Execute(AIAgent agent)
    {

        var walkedDistance = 0f;
        while (walkedDistance < maxDistance)
        {

            if (!(await CanMoveForward.EvaluateCondition(agent)))
                return;
            
            var delta = speed * Time.deltaTime;
            walkedDistance += delta;

            if (target == Target.Forward)
            {
                rigidbody.position += (animatorController.direction.ToVector2() * path.Evaluate(walkedDistance / maxDistance) * delta).ToVector2();
            }
            else if (target == Target.TowardsPlayer)
            {
                if (!isOnGrid)
                    rigidbody.position += (PlayerDirection * path.Evaluate(walkedDistance / maxDistance) * delta);
                else
                {
                    //TODO: Implement walking on grid
                }
            }

            await NextFrame;

        }

    }

    Vector2 PlayerDirection => (PlayerTracker.Transform.position - transform.position).normalized;

}
