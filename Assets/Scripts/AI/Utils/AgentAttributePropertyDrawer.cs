using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class AgentAttribute : PropertyAttribute
{ }

#if UNITY_EDITOR
[CustomPropertyDrawer(typeof(AgentAttribute))]
public class AgentAttributePropertyDrawer : PropertyDrawer
{

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    { }

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        return 0;
    }

}
#endif
