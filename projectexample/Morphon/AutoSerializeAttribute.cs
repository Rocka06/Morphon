using System;

[AttributeUsage(AttributeTargets.Class)]
public class AutoSerializableAttribute : Attribute
{
    public string TypeName { get; }
    public AutoSerializableAttribute(string typeName) => TypeName = typeName;
}
