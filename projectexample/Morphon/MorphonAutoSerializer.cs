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
        if (dictData == null)
        {
            GD.PrintErr("Invalid data!");
            return null;
        }

        if (!dictData.ContainsKey("Type"))
        {
            GD.PrintErr("Type was not set in serialized data!");
            return null;
        }

        string type = dictData["Type"].As<string>();
        if (!_typeMap.ContainsKey(type))
        {
            GD.PrintErr("Unregistered type: ", type);
            return null;
        }

        IMorphonSerializable obj = (IMorphonSerializable)Activator.CreateInstance(_typeMap[type]);
        obj.Deserialize(dictData);
        return obj;
    }

    /// <summary>
    /// This function deserializes list data into any IMoprhonSerializable[] type
    /// </summary>
    /// <param name="data">It should be an Array of Dictionaries of Variants</param>
    /// <returns>A list of IMorphonSerializable objects</returns>
    public static IMorphonSerializable[] DeserializeList(Variant data)
    {
        System.Collections.Generic.List<IMorphonSerializable> objList = new();
        Array<Dictionary<string, Variant>> arrayData = data.As<Array<Dictionary<string, Variant>>>();
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
            obj.Serialize(out Dictionary<string, Variant> data);
            data.Add("Type", obj.GetType().FullName);
            objArray.Add(data);
        }

        return objArray;
    }
}
