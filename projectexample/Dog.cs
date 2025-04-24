using System.Linq;
using Godot;
using Morphon;

[GlobalClass]
public partial class Dog : AnimalResource
{
    [Export] public Cat[] CatFriends;

    public override void Deserialize(string jsonData)
    {
        base.Deserialize(jsonData);
        Name = m_SerializerDict["Name"].As<string>();
        Age = m_SerializerDict["Age"].As<int>();
        CatFriends = MorphonAutoSerializer.DeserializeList(m_SerializerDict["CatFriends"].As<string>()).Cast<Cat>().ToArray();
    }

    public override string Serialize()
    {
        base.Serialize();
        m_SerializerDict.Add("CatFriends", MorphonAutoSerializer.SerializeList(CatFriends));

        return Json.Stringify(m_SerializerDict);
    }

    public override string ToString()
    {
        string baseString = base.ToString();
        return baseString + $"\nCatFriend: {CatFriends[0].Name}";
    }
}
