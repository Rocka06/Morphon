using System;
using System.Collections.Generic;
using System.Reflection;
using Godot;

namespace Morphon;
public static class MorphonAutoSerializer
{
    private static readonly Dictionary<string, Type> _typeMap = new();

    static MorphonAutoSerializer()
    {
        RegisterTypes();
    }

    private static void RegisterTypes()
    {
        Type baseType = typeof(IMorphonSerializable);
        foreach (Type type in Assembly.GetExecutingAssembly().GetTypes())
        {
            if (!baseType.IsAssignableFrom(type) || type.IsAbstract) continue;
            _typeMap[type.FullName] = type;
        }
    }

    public static IMorphonSerializable Deserialize(string json)
    {
        Godot.Collections.Dictionary dict = Json.ParseString(json).As<Godot.Collections.Dictionary>();
        
        if (dict == null) return null;
        if (!dict.ContainsKey("Type"))
        {
            GD.PrintErr("Type was not set in serialized data!");
            return null;
        }

        string type = dict["Type"].As<string>();
        if (!_typeMap.ContainsKey(type))
        {
            GD.PrintErr("Unregistered type: ", type);
            return null;
        }

        IMorphonSerializable obj = (IMorphonSerializable)Activator.CreateInstance(_typeMap[type]);
        obj.Deserialize(json);
        return obj;
    }

    public static List<IMorphonSerializable> DeserializeList(string jsonArray)
    {
        Godot.Collections.Array<string> array = Json.ParseString(jsonArray).As<Godot.Collections.Array<string>>();
        List<IMorphonSerializable> objList = new();

        foreach (var item in array)
        {
            IMorphonSerializable obj = Deserialize(item.ToString());
            if (obj != null) objList.Add(obj);
        }

        return objList;
    }

    public static string SerializeList(IEnumerable<IMorphonSerializable> list)
    {
        Godot.Collections.Array<string> array = new();

        foreach (IMorphonSerializable obj in list)
        {
            array.Add(obj.Serialize());
        }

        return array.ToString();
    }
}
