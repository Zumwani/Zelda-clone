using System;
using System.Threading.Tasks;

[Serializable]
public abstract class AICondition : AIComponent
{

    protected abstract bool Evaluate(AIAgent agent);

    public Task<bool> EvaluateCondition(AIAgent agent)
    {
        agent.action = GetType().Name.Replace("Condition", "");
        return new Task<bool>(() => Evaluate(agent));
    }

}
