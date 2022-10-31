namespace AnQL.Core.Attributes;

[AttributeUsage(AttributeTargets.Property)]
public class AnQLPropertyAttribute : Attribute
{
    public string? Name { get; set; }
    
    public AnQLPropertyAttribute()
    {
    }
    
    public AnQLPropertyAttribute(string name)
    {
        Name = name;
    }
}
