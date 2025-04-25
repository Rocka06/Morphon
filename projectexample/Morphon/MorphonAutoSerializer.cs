using System;
using System.Reflection;
using Godot;
using Godot.Collections;

namespace Morphon;
public static class MorphonAutoSerializer
{
    private static readonly System.Collections.Generic.Dictionary<string, Type> _typeMap = new();

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
        Dictionary<string, Variant> dict = Json.ParseString(json).As<Dictionary<string, Variant>>();
        
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
        obj.Deserialize(dict);
        return obj;
    }

    public static System.Collections.Generic.List<IMorphonSerializable> DeserializeList(string jsonArray)
    {
        Array<string> array = Json.ParseString(jsonArray).As<Array<string>>();
        System.Collections.Generic.List<IMorphonSerializable> objList = new();

        foreach (string item in array)
        {
            IMorphonSerializable obj = Deserialize(item.ToString());
            if (obj != null) objList.Add(obj);
        }

        return objList;
    }

    public static Array<Dictionary<string, Variant>> SerializeList(System.Collections.Generic.IEnumerable<IMorphonSerializable> list)
    {
        Array<Dictionary<string, Variant>> objArray = new();

        foreach (IMorphonSerializable obj in list)
        {
            obj.Serialize(out Dictionary<string, Variant> data);
            data.Add("Type", obj.GetType().FullName);
            objArray.Add(data);
        }

        return objArray;
    }
}
