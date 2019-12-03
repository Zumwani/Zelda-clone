using System.Reflection;

public static class TypeExtensions
{

    public static T GetAttribute<T>(this System.Type type, bool inherit = false) where T : System.Attribute 
    {
        return type.GetCustomAttribute(typeof(T), inherit) as T;
    }

}
