#if UNITY_EDITOR
using System;
using System.Linq;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(AIBehaviour))]
public class AIBehaviourEditor : Editor
{

    static Dictionary<Type, GUIContent> Conditions = new Dictionary<Type, GUIContent>();
    static Dictionary<Type, GUIContentStruct> Actions = new Dictionary<Type, GUIContentStruct>();

    #region Cached styles / texts / menus
    
    static class Styles
    {
        public static readonly GUIStyle collection = new GUIStyle(EditorStyles.helpBox) { margin = new RectOffset(12, 12, 6, 12) };
        public static readonly GUIStyle offsetBoldLabel = new GUIStyle(EditorStyles.boldLabel) { padding = new RectOffset(12, 0, 0, 0) };
        public static readonly GUIStyle addButton = new GUIStyle(GUI.skin.button) { margin = new RectOffset(0, 12, 0, 0) };
    }

    static class Texts
    {
        public static readonly GUIContent conditional = new GUIContent("Conditional:");
        public static readonly GUIContent condition = new GUIContent("Condition:");
        public static GUIContent ifFalse = new GUIContent("If false:");
        public static GUIContent ifTrue = new GUIContent("If true:");
        public static GUIContent menu = new GUIContent("⋮");
        public static GUIContent add = new GUIContent("Add");
        public static GUIContent none = new GUIContent("None");
    }

    public struct GUIContentStruct
    {

        public GUIContentStruct(string text, string tooltip = "")
        {
            normal = new GUIContent(text, tooltip);
            withColon = new GUIContent(text + ":", tooltip);
        }

        public GUIContent normal;
        public GUIContent withColon;

        public string text => normal.text;

        public static implicit operator GUIContent(GUIContentStruct s)
        {
            return s.normal;
        }

    }
     
    static class Menu
    {

        public static void Show(AIActionCollection collection, string path = "")
        {
            var menu = new GenericMenu();
            AddAction(menu, path, collection.Add);
            menu.ShowAsContext();
        }

        public static void Show(AIAction action, AIActionCollection collection)
        {

            var menu = new GenericMenu();

            menu.AddItem(new GUIContent("Remove"), false, () => collection.Remove(action));

            menu.AddSeparator(string.Empty);

            AddAction(menu, "Create new above", (a) => collection.AddAbove(action, a));
            AddAction(menu, "Create new below", (a) => collection.AddBelow(action, a));

            menu.ShowAsContext();

        }

        public static void AddAction(GenericMenu menu, string path, Action<Type> callback)
        {

            if (path != "" && !path.EndsWith("/"))
                path += "/";

            menu.AddItem(new GUIContent(path + "Conditional"), false, () => callback?.Invoke(typeof(AIConditional)));
            menu.AddSeparator(path);

            foreach (var action in Actions)
                menu.AddItem(new GUIContent(path + action.Value.text), false, () => callback.Invoke(action.Key));

        }


    }

    #endregion

    AIBehaviour target => (AIBehaviour)serializedObject.targetObject;

    Vector2 scroll;

    private void OnEnable()
    {

        foreach (var type in GetType().Assembly.ExportedTypes)
        {

            if (typeof(AIAction) == type || typeof(AIConditional) == type || typeof(AICondition) == type)
                continue;

            if (typeof(AICondition).IsAssignableFrom(type))
                Conditions.Add(type, new GUIContent(ObjectNames.NicifyVariableName(type.Name.Replace("Condition", ""))));

            else if (typeof(AIAction).IsAssignableFrom(type))
                Actions.Add(type, new GUIContentStruct(ObjectNames.NicifyVariableName(type.Name.Replace("Action", ""))));

        }

    }

    public override void OnInspectorGUI()
    {
         
        scroll = GUI.BeginScrollView(new Rect(0, 0, Screen.width, Screen.height), scroll, new Rect(0, 0, Screen.width, Screen.height - 12));

        if (target.actions == null)
            target.actions = new AIActionCollection(target);

        EditorGUI.BeginChangeCheck();

        Draw(target.actions);

        if (EditorGUI.EndChangeCheck())
        {
            Undo.RegisterCompleteObjectUndo(target, "Changed behaviour");
            target.actions = target.actions;
            EditorUtility.SetDirty(target);

            UnityEditor.SceneManagement.EditorSceneManager.MarkAllScenesDirty();
            Undo.FlushUndoRecordObjects();
        }

        AddButton();
        GUI.EndScrollView();

    }

    void AddButton()
    {
        GUILayout.BeginHorizontal();
        GUILayout.FlexibleSpace();
        if (GUILayout.Button(Texts.add, Styles.addButton, GUILayout.ExpandWidth(false)))
            Menu.Show(target.actions);
        GUILayout.EndHorizontal();
    }

    void Draw(AIActionCollection actions)
    {

        if (actions == null)
            actions = new AIActionCollection(target);
        actions.Parent = target;

        GUILayout.BeginVertical(Styles.collection);

        if (actions?.Count > 0)
            foreach (var action in actions)
                Draw(action, actions);
        else
            EditorGUILayout.LabelField("-- No actions --");
        GUILayout.EndVertical();

    }

    void Draw(AIAction action, AIActionCollection collection)
    {

        Header(action is AIConditional ? Texts.conditional : Actions[action.GetType()].withColon, action, collection);
        if (action is AIConditional c)
            Draw(c);
        else
            Draw(action);
        
    }

    void Draw(AIConditional conditional)
    {

        if (!conditional)
            return;

        ConditionField(conditional);
        Draw(conditional.condition);

        if (conditional.ifFalse == null) conditional.ifFalse = new AIActionCollection();
        if (conditional.ifTrue == null)  conditional.ifTrue  = new AIActionCollection();

        Header(Texts.ifTrue, conditional.ifTrue);
        Draw(conditional.ifTrue);

        Header(Texts.ifFalse, conditional.ifFalse);
        Draw(conditional.ifFalse);

    }

    void ConditionField(AIConditional conditional)
    {

        var i = conditional.condition ? Conditions.Keys.ToList().IndexOf(conditional.condition.GetType()) : -1;
        EditorGUI.BeginChangeCheck();
        var l = Conditions.Values.ToList();
        l.Insert(0, Texts.none);
        i = EditorGUILayout.Popup(Texts.condition, i + 1, l.ToArray());

        if (EditorGUI.EndChangeCheck())
            if (i == 0)
                conditional.condition = null;
            else
                conditional.condition = (AICondition)Activator.CreateInstance(Conditions.Keys.ElementAt(i - 1));

    }

    void Draw(AIAction action)
    {

        if (action == null)
            return;

        var obj = new SerializedObject(action);
        obj.Update();
        DrawPropertiesExcluding(obj, "m_Script");
        obj.ApplyModifiedProperties();

    }

    void Draw(AICondition condition)
    {

        if (condition == null)
            return;

        var obj = new SerializedObject(condition);
        obj.Update();
        DrawPropertiesExcluding(obj, "m_Script");
        obj.ApplyModifiedProperties();

    }

    #region Draw header

    void Header(GUIContent label, AIActionCollection collection)
    {
        GUILayout.BeginHorizontal();
        EditorGUILayout.LabelField(label, Styles.offsetBoldLabel);
        if (GUILayout.Button(Texts.menu, GUIStyle.none, GUILayout.ExpandWidth(false)))
            Menu.Show(collection);
        GUILayout.EndHorizontal();
    }

    void Header(GUIContent label, AIAction action, AIActionCollection collection)
    {
        GUILayout.BeginHorizontal();
        EditorGUILayout.LabelField(label, EditorStyles.boldLabel);
        if (GUILayout.Button(Texts.menu, GUIStyle.none, GUILayout.ExpandWidth(false)))
            Menu.Show(action, collection);
        GUILayout.EndHorizontal();
    }
    
    #endregion

}
#endif
