using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

[Serializable]
public class AIActionCollection : IEnumerable<AIAction>
{

    public async Task Evaluate(AIAgent agent, CancellationToken token)
    {

        foreach (var action in actions)
        {

            if (token.IsCancellationRequested)
                return;

            if (action is AIConditional c)
                await c.ExecuteConditional(agent, token);
            else
                await action.ExecuteAction(agent);

        }

    }

    public AIActionCollection() { }

    public AIActionCollection(AIBehaviour parent) =>
        this.parent = parent;

    [SerializeField] AIBehaviour parent;
    [SerializeField] List<AIAction> actions = new List<AIAction>();

    public AIAction this[int index] => actions[index];
    public AIBehaviour Parent
    {
        get => parent;
        internal set => parent = value;
    }

    public void Add(Type type)
    {
        if (IsValid(type))
            actions.Add(Create(type));
    }

    public void AddAbove(AIAction action, Type type)
    {

        if (!IsValid(type))
            return;

        var i = actions.IndexOf(action);
        if (i < 1) i = 1;
        actions.Insert(i - 1, Create(type));
    }

    public void AddBelow(AIAction action, Type type)
    {

        if (!IsValid(type))
            return;

        var i = actions.IndexOf(action);
        if (i < actions.Count)
            actions.Insert(i + 1, Create(type));
        else
            actions.Add(Create(type));

    }

    public void Remove(AIAction action)
    {
        parent.Remove(action);
        actions.Remove(action);
    }

    bool IsValid(Type type) =>
        typeof(AIAction).IsAssignableFrom(type) && type != typeof(AIAction);

    AIAction Create(Type type) =>
        (AIAction)parent.Create(type);

    public IEnumerator<AIAction> GetEnumerator() =>
        ((IEnumerable<AIAction>)actions).GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() => 
        ((IEnumerable<AIAction>)actions).GetEnumerator();

    public int Count => actions.Count;

}
