using Godot;
using Morphon;

[GlobalClass]
public partial class AnimalResource : Resource, IMorphonSerializable
{
    [Export] public string Name;
    [Export] public int Age;

    protected Godot.Collections.Dictionary<string, Variant> m_SerializerDict;

    public virtual void Deserialize(string jsonData)
    {
        m_SerializerDict = Json.ParseString(jsonData).As<Godot.Collections.Dictionary<string, Variant>>();
        Name = m_SerializerDict["Name"].As<string>();
        Age = m_SerializerDict["Age"].As<int>();
    }

    public virtual string Serialize()
    {
        m_SerializerDict = new()
        {
            { "Type", GetType().FullName },
            { "Name", Name },
            { "Age", Age }
        };

        return Json.Stringify(m_SerializerDict);
    }

    public override string ToString()
    {
        return $"Name: {Name}\nAge: {Age}";
    }
}
