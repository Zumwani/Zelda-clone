using UnityEngine;
using System.Linq;
using System.Reflection;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class ShowIfAttribute : PropertyAttribute
{

    public string propertyName;
    public object value;
    public bool useValue;
    public bool invert;

    public ShowIfAttribute(string name)
    {
        propertyName = name;
        useValue = false;
    }

    public ShowIfAttribute(string name, object value = null)
    {
        propertyName = name;
        this.value = value;
        useValue = true;
    }

}

public class ArrayItemNameAttribute : PropertyAttribute
{
    public string propertyName;
    public bool alwaysUpdate;
    public ArrayItemNameAttribute(string name)
    {
        propertyName = name;
    }
}

public class HiddenAttribute : PropertyAttribute
{ }

public class LabelAttribute : PropertyAttribute
{ }

public enum GetComponentDirection
{
    Self, Parent, Children
}

public class AutoAttribute : PropertyAttribute
{

    public bool useCameraMain = true;
    public GetComponentDirection direction;

}

public class RequiredAttribute : AutoAttribute
{ }

public class ButtonAttribute : PropertyAttribute
{

    public string function;

    public ButtonAttribute(string function)
    { this.function = function; }

}

public class ToggleButtonAttribute : ShowIfAttribute
{

    public string function;
    public string on;
    public string off;

    public ToggleButtonAttribute(string name, string on, string off, string function) : base(name)
    { 
        this.function = function;
        this.on = on;
        this.off = off;
    }

    public ToggleButtonAttribute(string name, object value, string on, string off, string function) : base(name, value)
    {
        this.function = function;
        this.on = on;
        this.off = off;
    }

}

#if UNITY_EDITOR

[CustomPropertyDrawer(typeof(ToggleButtonAttribute), false)]
public class ToggleButtonPropertyDrawer : ShowIfPropertyDrawer
{

    public override void OnDrawField(bool result, Rect position, SerializedProperty property, GUIContent label)
    {

        if (property.propertyType != SerializedPropertyType.Boolean)
            return;

        var attr = (ToggleButtonAttribute)attribute;
        property.boolValue = GUI.Button(position, result ? attr.on : attr.off);
        if (property.boolValue)
            property.serializedObject.targetObject.GetType().GetMethod(attr.function)?.Invoke(property.serializedObject.targetObject, new object[] { result });

    }

}

[CustomPropertyDrawer(typeof(ShowIfAttribute), false)]
public class ShowIfPropertyDrawer : PropertyDrawer<ShowIfAttribute>
{

    const BindingFlags bindingFlags =
        BindingFlags.Public | BindingFlags.NonPublic |
        BindingFlags.Static | BindingFlags.Instance |
        BindingFlags.GetField | BindingFlags.GetProperty | BindingFlags.InvokeMethod;

    bool isTrue = false;
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {

        var obj = property.GetParent();
        if (obj == null)
            return;

        isTrue = Eval(obj, attribute);
        OnDrawField(isTrue, position, property, label);

    }

    public virtual void OnDrawField(bool result, Rect position, SerializedProperty property, GUIContent label)
    {
        if (result)
            EditorGUI.PropertyField(position, property);
    }

    public static bool Eval(object obj, ShowIfAttribute attribute)
    {

        bool isTrue = false;
        var member = obj.GetType().GetMember(attribute.propertyName, bindingFlags).FirstOrDefault();

        if (member == null)
            isTrue = true;
        else if (member.GetValue(obj) is object value)
            if (attribute.useValue)
                isTrue = Equals(value, attribute.value);
            else if (value is bool)
                isTrue = (bool)value == true;
            else if (value is Object)
                isTrue = (Object)value;
            else if (value != null)
                isTrue = true;

        if (attribute.invert)
            isTrue = !isTrue;

        return isTrue;

    }

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        if (isTrue)
            return base.GetPropertyHeight(property, label);
        else
            return 0;
    }

}

[CustomPropertyDrawer(typeof(ArrayItemNameAttribute))]
public class ArrayItemNamePropertyDrawer : PropertyDrawer<ArrayItemNameAttribute>
{

    const BindingFlags bindingFlags =
          BindingFlags.Public   | BindingFlags.NonPublic   |
          BindingFlags.Static   | BindingFlags.Instance    |
          BindingFlags.GetField | BindingFlags.GetProperty | BindingFlags.InvokeMethod;

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {

        if (!string.IsNullOrWhiteSpace(property.stringValue) && !attribute.alwaysUpdate)
            return;

        var obj = property.GetParent();
        if (obj == null)
            return;

        var member = obj.GetType().GetMember(attribute.propertyName, bindingFlags).FirstOrDefault();

        if (member.GetValue(obj) is string name)
            property.stringValue = name;

    }

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        return 0;
    }

}

[CustomPropertyDrawer(typeof(HiddenAttribute))]
public class HiddenPropertyDrawer : PropertyDrawer<HiddenAttribute>
{

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    { }

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        return 0;
    }

}

[CustomPropertyDrawer(typeof(ButtonAttribute))]
public class ButtonDrawer : PropertyDrawer<ButtonAttribute>
{

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {

        if (property.propertyType != SerializedPropertyType.Boolean)
            return;

        property.boolValue = GUI.Button(position, label);
        if (property.boolValue)
            property.serializedObject.targetObject.GetType().GetMethod(attribute.function)?.Invoke(property.serializedObject.targetObject, System.Array.Empty<object>());

    }

}

[CustomPropertyDrawer(typeof(LabelAttribute))]
public class LabelDrawer : PropertyDrawer<LabelAttribute>
{

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        EditorGUI.LabelField(position, label, new GUIContent(fieldInfo.GetValue(property.serializedObject.targetObject).ToString()));
    }

}

[CustomPropertyDrawer(typeof(RequiredAttribute))]
[CustomPropertyDrawer(typeof(AutoAttribute))]
public class AutoDrawer : PropertyDrawer
{

    static readonly GUIStyle redStyle = new GUIStyle() { normal = new GUIStyleState() { textColor = Color.red } };
    static readonly GUIContent missing = new GUIContent("Missing:");

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {

        if (property.propertyType != SerializedPropertyType.ObjectReference)
            return;

        //Fix for when user changes type, which will result in type mismatch

        bool hasValue = property.objectReferenceValue;
        var isTypeMismatch = hasValue && fieldInfo.FieldType != property.objectReferenceValue.GetType();

        if (hasValue && !isTypeMismatch)
            return;

        if (((AutoAttribute)attribute).useCameraMain && fieldInfo.FieldType == typeof(Camera))
            property.objectReferenceValue = Camera.main;
        else
            switch (((AutoAttribute)attribute).direction)
            {
                case GetComponentDirection.Self:
                    property.objectReferenceValue = ((Component)property.serializedObject.targetObject).GetComponent(fieldInfo.FieldType);
                    break;
                case GetComponentDirection.Parent:
                    property.objectReferenceValue = ((Component)property.serializedObject.targetObject).GetComponentInParent(fieldInfo.FieldType);
                    break;
                case GetComponentDirection.Children:
                    property.objectReferenceValue = ((Component)property.serializedObject.targetObject).GetComponentInChildren(fieldInfo.FieldType);
                    break;
            }
       
        if (attribute is RequiredAttribute && !property.objectReferenceValue)
            EditorGUI.LabelField(position, missing, new GUIContent(fieldInfo.FieldType.Name), redStyle);
        
    }

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        if (attribute is RequiredAttribute && !property.objectReferenceValue)
            return base.GetPropertyHeight(property, label);
        else
            return 0;
    }

}

#endif
