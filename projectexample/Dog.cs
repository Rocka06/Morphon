using System.Linq;
using Godot;
using Morphon;
using Godot.Collections;

[GlobalClass]
public partial class Dog : AnimalResource
{
    [Export] public Cat[] CatFriends;

    public override void Deserialize(Dictionary<string, Variant> data)
    {
        base.Deserialize(data);
        Name = data["Name"].As<string>();
        Age = data["Age"].As<int>();
        CatFriends = MorphonAutoSerializer.DeserializeList(data["CatFriends"].As<string>()).Cast<Cat>().ToArray();
    }

    public override Dictionary<string, Variant> Serialize()
    {
        base.Serialize();
        m_SerializerDict.Add("CatFriends", MorphonAutoSerializer.SerializeList(CatFriends));

        return m_SerializerDict;
    }

    public override string ToString()
    {
        string baseString = base.ToString();
        return baseString + $"\nCatFriend: {CatFriends[0].Name}";
    }
}
