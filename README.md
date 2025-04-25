# Morphon

A Plug-and-Play **ConfigFile-style** system for Godot 4 C# that supports saving and loading **custom Resources** and Json supported values into Json files!  

---

## Features

- ✅ Works like Godot's `ConfigFile` but without the risk of injections
- ✅ Serialize and deserialize single values or entire lists
- ✅ Built with Godot's native `JSON`, `Dictionary`, and `Array` types — no external dependencies

---

## ✨ How to use

### Create custom Resources that implement the `IMorphonSerializable` interface

#### Serialization is basically just adding Json supported types to a Dictionary
If a value is not supported by Json (Resources):
- If it implements the IMorphonSerializable interface then you can use MorphonAutoSerializer.Serialize(object) to deserialize the object.
- If it's a list of IMorphonSerializable objects then you can use `MorphonAutoSerializer.SerializeList(list)` to serialize the list.
- If it's a built in Resource and is not local to scene (for example: SpriteFrames) then you can just add it to the dictionary and it have it's path saved!
- Otherwise it cannot be saved properly, and a value of `null` will be written in the save file

#### Deserialization is basically just reading data from a Dictionary of Variants
If a value you are trying to deserialize is not supported by Json (Resources):
- If it implements the IMorphonSerializable interface then you can use MorphonAutoSerializer.Deserialize(object) to deserialize the object.
- If it's a list of IMorphonSerializable objects then you can use `MorphonAutoSerializer.DeserializeList(list)` to deserialize the list.
- If it's a built in Resource and was not local to scene (for example: SpriteFrames) then you can just parse the Variant with `.As<SpriteFrames>()`!
- Otherwise it cannot be saved properly, and a value of `null` will be written in the save file

Pro tip: If you make the `IMorphonSerializable`s functions virtual then you don't have to reserialize the base class!

```csharp
using Godot;
using Godot.Collections;

[GlobalClass]
public partial class Animal : Resource, IMorphonSerializable
{
    [Export] public string Name;
    [Export] public int Age;

    public virtual void Serialize(out Dictionary<string, Variant> data)
    {
        data = new()
        {
            { "Name", Name },
            { "Age", Age }
        };
    };

    public virtual void Deserialize(Dictionary<string, Variant> data)
    {
        Name = data["Name"].As<string>();
        Age = data["Age"].As<int>();
    }
}
```

```csharp
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
```

```csharp
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
```

---

### 🔧 How to use `MorphonConfigFile`

You can use the MorphonConfigFile just like Godots built in ConfigFile!

```csharp
using Godot;
using Morphon;

public partial class Test : Node
{
    [Export] public AnimalResource animal;

    private MorphonConfigFile file = new();

    public override void _Ready()
    {
        file.SetValue("Data", "Animal", (IMorphonSerializable)animal);
        file.Save("user://Save.json");

        file.Clear();
        file.Load("user://Save.json");

        GD.Print("Loaded data:");
        GD.Print(file.GetValue<AnimalResource>("Data", "Animal"));
    }
}
```

#### Printed data example:
```
Loaded data:
Name: Doggo
Age: 1
CatFriend: Kitty
Icon path: res://icon.svg
DogFriend: Pongo
```

### 💾 File Format Example

```JSON
{
  "Animal": {
    "Animal": {
      "Age": 1,
      "CatFriends": [
        {
          "Age": 3,
          "Name": "Kitty",
          "Type": "Cat",
          "color": "ffff00ff"
        }
      ],
      "DogFriend": {
        "Age": 8,
        "CatFriends": [],
        "Name": "Pongo",
        "Type": "Dog"
      },
      "Icon": "res://icon.svg",
      "Name": "Doggo",
      "Type": "Dog"
    }
  }
}
```

---

## 📃 License

MIT — use it freely for commercial or personal projects.