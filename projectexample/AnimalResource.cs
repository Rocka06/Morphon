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
    public virtual void Serialize(out Dictionary<string, Variant> data)
    {
        data = new()
        {
            { "Name", Name },
            { "Age", Age }
        };
    }

    public override string ToString()
    {
        return $"Name: {Name}\nAge: {Age}";
    }
}
