namespace Common.Attributes;

[AttributeUsage(AttributeTargets.Enum)]
public class PublicEnumAttribute(string description) : Attribute
{
    public string Description { get; set; } = description;
}
