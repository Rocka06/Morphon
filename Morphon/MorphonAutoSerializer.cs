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

    /// <summary>
    /// This function deserializes data into any IMoprhonSerializable type
    /// </summary>
    /// <param name="data">It should be a Dictionary of Variants</param>
    /// <returns>IMorphonSerializable object</returns>
    public static IMorphonSerializable Deserialize(Variant data)
    {
        Dictionary<string, Variant> dictData = data.As<Dictionary<string, Variant>>();

        //This is needed so that we don't override the original
        Dictionary<string, Variant> newDict = new();

        if (dictData == null)
        {
            GD.PrintErr("Invalid data!");
            return null;
        }

        if (!dictData.ContainsKey("Type"))
        {
            GD.PrintErr($"Type was not set in serialized data!\nData: {dictData}");
            return null;
        }

        string type = dictData["Type"].As<string>();
        if (!_typeMap.ContainsKey(type))
        {
            GD.PrintErr("Unregistered type: ", type);
            return null;
        }

        //Check for paths and load them back
        foreach (var pair in dictData)
        {
            if (pair.Value.As<string>().StartsWith("res://"))
            {
                newDict.Add(pair.Key, SafeLoadResourceFromPath<Variant>(pair.Value.As<string>()));
            }
            else
            {
                newDict.Add(pair.Key, pair.Value);
            }
        }

        IMorphonSerializable obj = (IMorphonSerializable)Activator.CreateInstance(_typeMap[type]);
        obj.Deserialize(newDict);
        return obj;
    }

    /// <summary>
    /// Serializes the object with .Serialize() and adds the type of the object to the dictionary
    /// </summary>
    public static Dictionary<string, Variant> Serialize(IMorphonSerializable obj)
    {
        obj.Serialize(out var data);
        data.Add("Type", obj.GetType().FullName);

        //Check for resources and save their path
        foreach (var pair in data)
        {
            if (pair.Value.VariantType == Variant.Type.Nil) continue;

            Resource rValue = pair.Value.As<Resource>();
            if (rValue != null)
            {
                data[pair.Key] = GetResourcePath(rValue);
            }
        }

        return data;
    }

    /// <summary>
    /// This function deserializes list data into any IMoprhonSerializable[] type
    /// </summary>
    /// <param name="data">It should be an Array of Dictionaries of Variants</param>
    /// <returns>A list of IMorphonSerializable objects</returns>
    public static IMorphonSerializable[] DeserializeList(Variant data)
    {
        System.Collections.Generic.List<IMorphonSerializable> objList = new();
        var arrayData = data.As<Array<Dictionary<string, Variant>>>();
        if (arrayData == null)
        {
            GD.PrintErr("Invalid list data!");
            return null;
        }

        foreach (Dictionary<string, Variant> item in arrayData)
        {
            IMorphonSerializable obj = Deserialize(item);
            if (obj != null) objList.Add(obj);
        }

        return objList.ToArray();
    }

    /// <summary>
    /// This function serrializes IMorphonSerializable lists into an Array of Dictionaries of Variants
    /// </summary>
    /// <param name="list">A list of any IMorphonSerializable type</param>
    /// <returns>An Array of Dictionaries of Variants</returns>
    public static Array<Dictionary<string, Variant>> SerializeList(System.Collections.Generic.IEnumerable<IMorphonSerializable> list)
    {
        Array<Dictionary<string, Variant>> objArray = new();

        foreach (IMorphonSerializable obj in list)
        {
            objArray.Add(Serialize(obj));
        }

        return objArray;
    }


    public static string GetResourcePath(Resource resource)
    {
        if (resource == null) return null;
        if (!resource.ResourceLocalToScene)
        {
            return resource.ResourcePath;
        }
        return null;
    }
    public static T SafeLoadResourceFromPath<[MustBeVariant] T>(string path, T @default = default)
    {
        if (!path.StartsWith("res://")) return @default;

        Variant obj = GD.Load(path);
        return obj.As<T>();
    }
}
