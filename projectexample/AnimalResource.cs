using Godot;
using Morphon;
using Godot.Collections;

[GlobalClass]
public partial class AnimalResource : Resource, IMorphonSerializable
{
    [Export] public string Name;
    [Export] public int Age;


    public virtual void Deserialize(Dictionary<string, Variant> data)
    {
        Name = data["Name"].As<string>();
        Age = data["Age"].As<int>();
    }

    protected Dictionary<string, Variant> m_SerializerDict;
    public virtual Dictionary<string, Variant> Serialize()
    {
        m_SerializerDict = new()
        {
            { "Type", GetType().FullName },
            { "Name", Name },
            { "Age", Age }
        };

        return m_SerializerDict;
    }

    public override string ToString()
    {
        return $"Name: {Name}\nAge: {Age}";
    }
}
