using System;
using System.Threading;
using System.Threading.Tasks;

[Serializable]
public class AIConditional : AIAction
{

    public AICondition condition;
    public AIActionCollection ifTrue;
    public AIActionCollection ifFalse;

    //AIConditionals are managed manually by AIActionCollection and will Execute(AIAgent, CancellationToken) instead.
    protected override void Execute(AIAgent agent)
    { }

    public async Task ExecuteConditional(AIAgent agent, CancellationToken token)
    {

        if (token.IsCancellationRequested)
            return;

        if (!condition || await condition.EvaluateCondition(agent))
            await ifTrue?.Evaluate(agent, token);
        else
            await ifFalse?.Evaluate(agent, token);
    
    }

}
