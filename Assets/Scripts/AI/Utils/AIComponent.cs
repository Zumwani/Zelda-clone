using System;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using UnityEngine;

[Serializable]
public class AIComponent : ScriptableObject
{

    [Agent]
    protected Transform transform;

    internal FieldInfo[] agentFields;

    internal void FindComponents(AIAgent agent)
    {

        if (agentFields == null)
            agentFields = GetType().GetFields().Where(IsValidField).ToArray();

        foreach (var field in agentFields)
            if (field.GetValue(this) == default)
                field.SetValue(this, agent.GetComponent(field.FieldType));

    }

    bool IsValidField(FieldInfo field)
    {
        return 
            field.GetCustomAttributes<AgentAttribute>().Any() &&
            typeof(Component).IsAssignableFrom(field.FieldType);
    }

    public static implicit operator bool(AIComponent component) =>
        !Equals(component, null);

    public Task NextFrame => TaskWaiters.NextFrame;

}
