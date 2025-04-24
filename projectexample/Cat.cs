using Godot;

[GlobalClass]
public partial class Cat : AnimalResource
{
    [Export] public Color color;

    public override void Deserialize(string jsonData)
    {
        base.Deserialize(jsonData);
        Name = m_SerializerDict["Name"].As<string>();
        Age = m_SerializerDict["Age"].As<int>();
        color = m_SerializerDict["color"].As<Color>();
    }

    public override string Serialize()
    {
        base.Serialize();
        m_SerializerDict.Add("color", color.ToHtml());

        return Json.Stringify(m_SerializerDict);
    }

    public override string ToString()
    {
        string baseString = base.ToString();
        return baseString + $"\nColor: {color}";
    }
}
