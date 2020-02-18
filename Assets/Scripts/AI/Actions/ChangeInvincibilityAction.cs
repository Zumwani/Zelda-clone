public class ChangeInvincibilityAction : AIAction
{

    [Agent]
    public Health health;

    public bool isInvincible;

    protected override void Execute(AIAgent agent)
    {
        health.isInvincible = isInvincible;
    }

}
