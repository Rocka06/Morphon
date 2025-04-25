using Godot;
using Godot.Collections;

[GlobalClass]
public partial class Cat : AnimalResource
{
    [Export] public Color color;

    public override void Deserialize(Dictionary<string, Variant> data)
    {
        base.Deserialize(data);
        Name = data["Name"].As<string>();
        Age = data["Age"].As<int>();
        color = data["color"].As<Color>();
    }

    public override Dictionary<string, Variant> Serialize()
    {
        base.Serialize();
        m_SerializerDict.Add("color", color.ToHtml());

        return m_SerializerDict;
    }

    public override string ToString()
    {
        string baseString = base.ToString();
        return baseString + $"\nColor: {color}";
    }
}
