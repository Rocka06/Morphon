using System;
using System.Collections.Generic;
using System.Reflection;
using Godot;
using Newtonsoft.Json.Linq;

public static class JsonAutoSerializer
{
    private static readonly Dictionary<string, Type> _typeMap = new();

    static JsonAutoSerializer()
    {
        RegisterTypes();
    }

    private static void RegisterTypes()
    {
        Type baseType = typeof(IResourceSerializable);
        foreach (Type type in Assembly.GetExecutingAssembly().GetTypes())
        {
            if (!baseType.IsAssignableFrom(type) || type.IsAbstract) continue;

            AutoSerializableAttribute attr = type.GetCustomAttribute<AutoSerializableAttribute>();
            if (attr != null)
            {
                _typeMap[attr.TypeName] = type;
            }
        }
    }

    public static IResourceSerializable Deserialize(string json)
    {
        JObject jo = JObject.Parse(json);
        string type = jo["Type"]?.ToString();
        if (type == null || !_typeMap.ContainsKey(type))
        {
            if (type == null)
            {
                GD.PrintErr("Type was not set in serialized data");
            }
            else
            {
                GD.PrintErr("Unregistered type: ", type);
            }
            return null;
        }

        IResourceSerializable obj = (IResourceSerializable)Activator.CreateInstance(_typeMap[type]);
        obj.Deserialize(json);
        return obj;
    }

    public static List<IResourceSerializable> DeserializeList(string jsonArray)
    {
        JArray jArray = JArray.Parse(jsonArray);
        List<IResourceSerializable> list = [];

        foreach (var item in jArray)
        {
            IResourceSerializable obj = Deserialize(item.ToString());
            if (obj != null) list.Add(obj);
        }

        return list;
    }

    public static string SerializeList(IEnumerable<IResourceSerializable> list)
    {
        JArray array = new();

        foreach (IResourceSerializable obj in list)
        {
            JObject jo = JObject.Parse(obj.Serialize());
            array.Add(jo);
        }

        return array.ToString();
    }
}
