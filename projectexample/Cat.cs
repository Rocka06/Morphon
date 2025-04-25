using Godot;
using Godot.Collections;

[GlobalClass]
public partial class Cat : AnimalResource
{
    [Export] public Color color;

    public override void Serialize(out Dictionary<string, Variant> data)
    {
        base.Serialize(out data); //This serializes the values from the base class

        //Then we just add the values from this class
        data.Add("color", color.ToHtml());
    }

    public override void Deserialize(Dictionary<string, Variant> data)
    {
        base.Deserialize(data); //This deserializes the values from the base class

        //Then we just deserialize the values from this class
        color = data["color"].As<Color>();
    }

    public override string ToString()
    {
        string baseString = base.ToString();
        return baseString + $"\nColor: {color}";
    }
}
