public class OctorockAnimationController : AnimationController<OctorockAnimationController.AnimationState>
{

    public enum AnimationState
    {
        Idle, Attacking
    }

    public SpritesSet idle;

    protected override void OnUpdate()
    {

        switch (State)
        {
            case AnimationState.Idle:
                renderer.sprite = idle.Direction(direction).GetSprite(renderer.sprite);
                break;
            default:
                break;
        }

    }

}
