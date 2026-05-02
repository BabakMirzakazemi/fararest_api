using Common.Markers;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Common.Utilities.Helpers;

public static class UtilConvertor
{
    public static T? ToObject<T>(dynamic input) where T : class, IBaseDTO
    {
        var obj = TryGetJObject((object?)input);
        if (obj is null || !obj.HasValues)
            return default;

        return obj.ToObject<T>();
    }

    public static T? ToObject<T>(string input) where T : class, IBaseDTO
    {
        if (string.IsNullOrWhiteSpace(input))
            return default;

        var obj = TryGetJObject((object?)JsonConvert.DeserializeObject<dynamic>(input));
        if (obj is null || !obj.HasValues)
            return default;

        return obj.ToObject<T>();
    }

    public static List<string> GetPropertiesName(dynamic input)
    {
        var obj = TryGetJObject((object?)input);
        if (obj is null || !obj.HasValues)
            return [];

        return obj.Properties().Select(p => p.Name).ToList();
    }

    public static List<string> GetPropertiesName(string input)
    {
        if (string.IsNullOrWhiteSpace(input))
            return [];

        var obj = TryGetJObject((object?)JsonConvert.DeserializeObject<dynamic>(input));
        if (obj is null || !obj.HasValues)
            return [];

        return obj.Properties().Select(p => p.Name).ToList();
    }

    public static bool ContainProperty(string input, string propertyName)
    {
        if (string.IsNullOrWhiteSpace(input) || string.IsNullOrWhiteSpace(propertyName))
            return false;

        var obj = TryGetJObject((object?)JsonConvert.DeserializeObject<dynamic>(input));
        if (obj is null || !obj.HasValues)
            return false;

        return obj.Properties().Any(p => p.Name.Equals(propertyName, StringComparison.OrdinalIgnoreCase));
    }

    public static bool ContainProperty(dynamic input, string propertyName)
    {
        if (string.IsNullOrWhiteSpace(propertyName))
            return false;

        var obj = TryGetJObject((object?)input);
        if (obj is null || !obj.HasValues)
            return false;

        return obj.Properties().Any(p => p.Name.Equals(propertyName, StringComparison.OrdinalIgnoreCase));
    }

    private static JObject? TryGetJObject(object? input)
    {
        return input switch
        {
            null => null,
            JObject jObject => jObject,
            JToken token => token as JObject,
            _ => null
        };
    }
}
