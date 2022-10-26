using System.Collections.Generic;
using AnQL.Core.Attributes;

namespace AnQL.Functions.Tests.Utilities;

public class DemoClass
{
    public string StringProperty { get; set; }
    
    [AnQLProperty]
    public int IntProperty { get; set; }
    public List<NestedDemoClass> NestedDemos { get; set; } = new();
}

public class NestedDemoClass
{
    public string StringProperty { get; set; }
    public string IntProperty { get; set; }
}