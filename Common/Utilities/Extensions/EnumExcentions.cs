using Common.Utilities.Helpers;
using System.ComponentModel.DataAnnotations;
using System.Reflection;

namespace Common.Utilities.Extensions;

public static class EnumExtensions
{
    public static IEnumerable<T> GetEnumValues<T>(this T input) where T : struct
    {
        if (!typeof(T).IsEnum)
            throw new NotSupportedException();

        return Enum.GetValues(input.GetType()).Cast<T>();
    }

    public static IEnumerable<T> GetEnumFlags<T>(this T input) where T : struct
    {
        if (!typeof(T).IsEnum)
            throw new NotSupportedException();

        var enumInput = input as Enum;
        if (enumInput is null)
            yield break;

        foreach (var value in Enum.GetValues(input.GetType()))
        {
            if (value is Enum enumValue && enumInput.HasFlag(enumValue))
                yield return (T)value;
        }
    }

    public static string ToDisplay(this Enum value, DisplayProperty property = DisplayProperty.Name)
    {
        Assert.NotNull(value, nameof(value));

        if (Convert.ToInt32(value) == 0)
            return value.ToString();

        var memberInfo = value.GetType().GetField(value.ToString());
        var attribute = memberInfo?.GetCustomAttributes<DisplayAttribute>(false).FirstOrDefault();

        if (attribute == null)
            return value.ToString();

        var propertyInfo = attribute.GetType().GetProperty(property.ToString());
        var propValue = propertyInfo?.GetValue(attribute, null);
        return propValue?.ToString() ?? value.ToString();
    }

    public static Dictionary<int, string> ToDictionary(this Enum value)
    {
        return Enum.GetValues(value.GetType()).Cast<Enum>().ToDictionary(p => Convert.ToInt32(p), q => q.ToDisplay());
    }

    public static T ParseEnum<T>(string value)
    {
        return (T)Enum.Parse(typeof(T), value, true);
    }

    public static T ToEnum<T>(this string? value)
    {
        if (!string.IsNullOrWhiteSpace(value))
            return (T)Enum.Parse(typeof(T), value, true);

        return default!;
    }
}

public enum DisplayProperty
{
    Description,
    GroupName,
    Name,
    Prompt,
    ShortName,
    Order
}
