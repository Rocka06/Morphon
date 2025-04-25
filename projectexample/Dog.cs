using System.Linq;
using Godot;
using Morphon;
using Godot.Collections;

[GlobalClass]
public partial class Dog : AnimalResource
{
    [Export] public Cat[] CatFriends;
    [Export] public Dog DogFriend;
    [Export] public Texture2D Icon;

    public override void Serialize(out Dictionary<string, Variant> data)
    {
        base.Serialize(out data);

        if (DogFriend != null)
        {
            data.Add("DogFriend", MorphonAutoSerializer.Serialize(DogFriend));
        }
        if (CatFriends != null)
        {
            data.Add("CatFriends", MorphonAutoSerializer.SerializeList(CatFriends));
        }
        if (Icon != null)
        {
            data.Add("Icon", Icon);
        }
    }
    public override void Deserialize(Dictionary<string, Variant> data)
    {
        base.Deserialize(data);

        //These checks allow the variables to be null
        if (data.ContainsKey("DogFriend"))
        {
            DogFriend = (Dog)MorphonAutoSerializer.Deserialize(data["DogFriend"]);
        }

        if (data.ContainsKey("CatFriends"))
        {
            IMorphonSerializable[] serializables = MorphonAutoSerializer.DeserializeList(data["CatFriends"]);
            CatFriends = serializables.Cast<Cat>().ToArray();
        }

        if (data.ContainsKey("Icon"))
        {
            Icon = data["Icon"].As<Texture2D>();
        }
    }

    public override string ToString()
    {
        string baseString = base.ToString();
        return baseString + $"\nCatFriend: {CatFriends[0].Name}\nIcon path: {Icon?.ResourcePath}\nDogFriend: {DogFriend.Name}";
    }
}
