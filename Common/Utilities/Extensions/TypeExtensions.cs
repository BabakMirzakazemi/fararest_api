using System.ComponentModel;
using System.Reflection;

namespace Common.Utilities.Extensions;

public static class TypeExtensions
{
    public static IEnumerable<Type> FindInterfacesThatClose(this Type pluggedType, Type templateType)
    {
        return FindInterfacesThatClosesCore(pluggedType, templateType).Distinct();
    }

    private static IEnumerable<Type> FindInterfacesThatClosesCore(Type? pluggedType, Type templateType)
    {
        if (pluggedType == null) yield break;

        if (!pluggedType.IsConcrete()) yield break;

        if (templateType.GetTypeInfo().IsInterface)
        {
            foreach (
                var interfaceType in
                pluggedType.GetInterfaces()
                    .Where(type => type.GetTypeInfo().IsGenericType && type.GetGenericTypeDefinition() == templateType))
            {
                yield return interfaceType;
            }
        }
        else if ((pluggedType.GetTypeInfo().BaseType?.GetTypeInfo().IsGenericType ?? false) &&
                 pluggedType.GetTypeInfo().BaseType?.GetGenericTypeDefinition() == templateType)
        {
            yield return pluggedType.GetTypeInfo().BaseType!;
        }

        if (pluggedType.GetTypeInfo().BaseType == typeof(object)) yield break;

        foreach (var interfaceType in FindInterfacesThatClosesCore(pluggedType.GetTypeInfo().BaseType, templateType))
        {
            yield return interfaceType;
        }
    }

    public static bool IsConcrete(this Type type)
    {
        return !type.GetTypeInfo().IsAbstract && !type.GetTypeInfo().IsInterface;
    }

    public static Dictionary<string, string> GetPropertyDisplayNamesDictionary(this Type type)
    {
        var result = new Dictionary<string, string>();
        var properties = type.GetProperties();

        foreach (var property in properties)
        {
            var displayNameAttribute = property.GetCustomAttribute<DisplayNameAttribute>();
            string displayName = displayNameAttribute != null ? displayNameAttribute.DisplayName : property.Name;
            result.Add(property.Name, displayName);
        }

        return result;
    }
}