using UnityEngine;
using System.Linq;

#if UNITY_EDITOR
using UnityEditor;
#endif

#if UNITY_EDITOR
[CustomEditor(typeof(Transform), true)]
public class TransformEditor : Editor
{

    static readonly GUIContent position = new GUIContent("Position");
    static readonly GUIContent X = new GUIContent("X");
    static readonly GUIContent Y = new GUIContent("Y");
    static readonly GUIContent content = new GUIContent("-100");

    public override void OnInspectorGUI()
    {

        var target = (Transform)base.target;

        EditorGUILayout.BeginHorizontal();

        EditorGUILayout.LabelField(position, GUILayout.Width(EditorStyles.label.CalcSize(position).x));
        GUILayout.FlexibleSpace();

        EditorGUI.BeginChangeCheck();

        var lockToGrid = EditorPrefs.GetBool("Transform.LockToGrid:" + target.name, true);

        if (!Application.isPlaying && lockToGrid)
            if (target.position.x % 1 != 0 || target.position.y % 1 != 0)
                target.position = new Vector2(Mathf.RoundToInt(target.position.x), Mathf.RoundToInt(target.position.y));

        var width = GUILayout.Width(EditorStyles.label.CalcSize(content).x + 8);

        EditorGUILayout.LabelField(X, GUILayout.Width(EditorStyles.label.CalcSize(X).x));
        
        var x = lockToGrid ? 
            EditorGUILayout.IntField((int)target.localPosition.x, width) : 
            EditorGUILayout.FloatField(target.localPosition.x, width);
       
        EditorGUILayout.LabelField(Y, GUILayout.Width(EditorStyles.label.CalcSize(Y).x));

        var y = lockToGrid ?
            EditorGUILayout.IntField((int)target.localPosition.y, width) :
            EditorGUILayout.FloatField(target.localPosition.y, width);

        if (EditorGUI.EndChangeCheck())
        {
            Undo.RecordObject(target, "Changed position");
            target.localPosition = new Vector2(x, y);
        }

        if (!directions.Keys.Contains(target.localEulerAngles.z))
            target.localEulerAngles = Vector2.up;

        var dir = directions[target.localEulerAngles.z];
        if (GUILayout.Button(dir, GUILayout.ExpandWidth(false)))
        {
            var i = directions.Values.ToList().IndexOf(dir);
            i += 1;
            if (i > directions.Count - 1) i = 0;
            if (i < 0) i = 0;
            Undo.RecordObject(target, "Changed rotation");
            target.localEulerAngles = new Vector3(0, 0, directions.ElementAt(i).Key);
        }

        EditorGUI.BeginChangeCheck();
        lockToGrid = EditorGUILayout.Toggle(lockToGrid, GUILayout.Width(12));
        if (EditorGUI.EndChangeCheck())
            EditorPrefs.SetBool("Transform.LockToGrid:" + target.name, lockToGrid);

        EditorGUILayout.EndHorizontal();
    
    }

    Dictionary<float, string> directions = new Dictionary<float, string>()
    {
        { 0, "↑" },
        { 270, "→" },
        { 90, "↓" },
        { 180, "←" },
    };

}
#endif
