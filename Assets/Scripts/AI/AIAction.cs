using System;
using System.Threading.Tasks;

[Serializable]
public abstract class AIAction : AIComponent
{

    protected abstract void Execute(AIAgent agent);

    public Task ExecuteAction(AIAgent agent)
    {
        agent.action = GetType().Name.Replace("Action", "");
        return new Task(() => Execute(agent));
    }

}
