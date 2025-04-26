# Morphon for Godot 4 (C#)

**Morphon** is a simple system for saving and loading structured object data in Godot 4 using Godot's built-in JSON.  
It acts like a `ConfigFile`, but supports **polymorphic serialization** through an interface and reflection.

---

## Features

- Interface-based design with `IMorphonSerializable`
- Supports JSON compatible Variants (number, string, bool, arrays, dictionaries), single objects and lists of objects
- No external libraries required (uses Godot's built-in JSON)
- Ideal for game saves, runtime configuration, and modular data

---

## How It Works

MorphonConfigFile works exactly like the built-in `ConfigFile` just without the risk of injections.  
Objects implementing `IMorphonSerializable` can define their own save/load logic.

---

## Example usage

### Defining Serializable Classes

Animal base class:
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
Inherited Cat class: 
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

Inherited Dog class: 
```csharp
using System.Linq;
using Godot;
using Morphon;
using Godot.Collections;

[GlobalClass]
public partial class Dog : AnimalResource
{
    [Export] public Cat CatFriend;
    [Export] public Texture2D Icon;

    public override void Serialize(out Dictionary<string, Variant> data)
    {
        base.Serialize(out data);

        if (CatFriend != null)
        {
            data.Add("CatFriend", MorphonAutoSerializer.Serialize(CatFriend));
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
        if (data.ContainsKey("CatFriend"))
        {
            CatFriend = (Cat)MorphonAutoSerializer.Deserialize(data["CatFriend"]);
        }

        if (data.ContainsKey("Icon"))
        {
            Icon = data["Icon"].As<Texture2D>();
        }
    }

    public override string ToString()
    {
        string baseString = base.ToString();
        return baseString + $"\nCatFriend: {CatFriend.Name}\nIcon path: {Icon?.ResourcePath}";
    }
}
```

---

### Saving Data

```csharp
MorphonConfigFile config = new();
config.SetValue("Player", "Pet", (IMorphonSerializable)new Dog()
{
    Name = "Doggo",
    Age = 1,
    CatFriend = new Cat()
    {
        Name = "Kitty",
        Age = 6,
        color = Color.Color8(200, 100, 43)
    }
});

config.SetValue("Game", "Cats", new List<Cat>
{
    new Cat { Name = "Whiskers", Age = 7 },
    new Cat { Name = "Kitty", Age = 2 }
});

config.Save("user://Save.json");
```

---

## Saved File Example

```json
{
  "Game": {
    "Cats": [
      {
        "Age": 7,
        "Name": "Whiskers",
        "Type": "Cat",
        "color": "00000000"
      },
      {
        "Age": 2,
        "Name": "Kitty",
        "Type": "Cat",
        "color": "00000000"
      }
    ]
  },
  "Player": {
    "Pet": {
      "Age": 1,
      "CatFriend": {
        "Age": 6,
        "Name": "Kitty",
        "Type": "Cat",
        "color": "c8642bff"
      },
      "Name": "Doggo",
      "Type": "Dog"
    }
  }
}
```

---

### Loading Data

```csharp
MorphonConfigFile config = new();
config.Load("user://Save.json");

Dog myDog = config.GetValue<Dog>("Player", "Pet");
List<Cat> cats = config.GetListValue<Cat>("Game", "Cats");
```

---

## Installation

1. Copy the `Morphon` folder into your Godot project.
2. You're all done.

---

## License

Licensed under the MIT License.