public class LinkAnimationController : AnimationController<LinkAnimationController.AnimationState>
{
    
    public enum AnimationState
    {
        Idle, Moving, Attacking
    }

    public SpritesSet move;
    public SpriteSet attack;

    protected override void OnUpdate()
    {

        switch (State)
        {
            case AnimationState.Moving:
                renderer.sprite = move.Direction(direction).GetSprite(renderer.sprite);
                break;
            case AnimationState.Attacking:
                renderer.sprite = attack.Direction(direction);
                break;
            default:
                break;
        }

    }

    public void NotifyMoving(bool isMoving)
    {
        if (isMoving)
            State = AnimationState.Moving;
        else if (State == AnimationState.Moving)
            State = AnimationState.Idle;
    }

}
