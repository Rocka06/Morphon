using System.Linq;
using Godot;
using Morphon;
using Godot.Collections;

[GlobalClass]
public partial class Dog : AnimalResource
{
    [Export] public Cat[] CatFriends;
    [Export] public Texture2D Icon;

    public override void Serialize(out Dictionary<string, Variant> data)
    {
        base.Serialize(out data);

        data.Add("CatFriends", MorphonAutoSerializer.SerializeList(CatFriends));
        data.Add("Icon", Icon);
    }
    public override void Deserialize(Dictionary<string, Variant> data)
    {
        base.Deserialize(data);

        IMorphonSerializable[] serializables = MorphonAutoSerializer.DeserializeList(data["CatFriends"]);
        CatFriends = serializables.Cast<Cat>().ToArray();

        Icon = data["Icon"].As<Texture2D>();
    }

    public override string ToString()
    {
        string baseString = base.ToString();
        return baseString + $"\nCatFriend: {CatFriends[0].Name}\n{Icon.ResourcePath}";
    }
}
