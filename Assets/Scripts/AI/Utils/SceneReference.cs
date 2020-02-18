using System;
using System.Linq;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class SceneReference : MonoBehaviour
{

    public string m_ID;
    public string ID =>
        !string.IsNullOrWhiteSpace(m_ID) ?
        m_ID :
        (m_ID = Guid.NewGuid().ToString());

    SceneReference()
    {
        _ = ID;
    }

    public static SceneReference Find(string id) =>
        FindObjectsOfType<SceneReference>().FirstOrDefault(o => o.ID == id);

    public static T Find<T>(string id) where T : Component
    {
        var obj = Find(id);
        if (obj)
            return obj.GetComponent<T>();
        else
            return null;
    }

}

public class SceneReferenceAttribute : PropertyAttribute
{ }

#if UNITY_EDITOR
[CustomPropertyDrawer(typeof(SceneReferenceAttribute))]
public class SceneReferencePropertyDrawer : PropertyDrawer
{

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {

        var obj = SceneReference.Find(property.stringValue);
        var newObj = EditorGUILayout.ObjectField(label, obj, typeof(SceneReference), true);

        if (newObj != obj)
            property.stringValue = newObj ? (newObj as SceneReference).ID : string.Empty;

    }

}
#endif
