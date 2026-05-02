using Newtonsoft.Json.Serialization;

namespace Common.Utilities.Helpers;

public class LowercaseContractResolver : DefaultContractResolver
{
    protected override string ResolvePropertyName(string propertyName)
    {
        return propertyName.ToLower();
    }
}
