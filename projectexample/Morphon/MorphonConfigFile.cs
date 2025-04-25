using System.Linq;
using Godot;
using Godot.Collections;

namespace Morphon;
public partial class MorphonConfigFile : Json
{
    private Dictionary<string, Dictionary<string, Variant>> m_Data = new();

    /// <summary>
    /// Sets a Variant value. If the variant is a Resource that is not local to scene, then it's path will be saved
    /// </summary>
    public void SetValue(string section, string key, Variant value)
    {
        NewKey(section, key);
        Resource rValue = value.As<Resource>();

        if (rValue != null)
        {
            m_Data[section][key] = GetResourcePath(rValue);
            return;
        }

        m_Data[section][key] = value;
    }

    /// <summary>
    /// Sets an IMorphonSerializable value. If the you Serialize a Resource that is not local to scene, then it's path will be saved
    /// </summary>
    public void SetValue(string section, string key, IMorphonSerializable value)
    {
        NewKey(section, key);
        value.Serialize(out var data);
        data.Add("Type", value.GetType().FullName);

        //Check for resource and save their path
        foreach (var pair in data)
        {
            if (pair.Value.VariantType == Variant.Type.Nil) continue;
            
            Resource rValue = pair.Value.As<Resource>();
            if (rValue != null)
            {
                data[pair.Key] = GetResourcePath(rValue);
            }
        }

        m_Data[section][key] = data;
    }
    public void SetValue(string section, string key, System.Collections.Generic.IEnumerable<IMorphonSerializable> list)
    {
        NewKey(section, key);
        m_Data[section][key] = MorphonAutoSerializer.SerializeList(list);
    }

    /// <summary>
    /// Gets a value by section and key. If the value is a Resource that was not local to scene and is in res:// then it will be loaded back.
    /// </summary>
    public T GetValue<[MustBeVariant] T>(string section, string key, T @default = default)
    {
        if (!HasSectionKey(section, key)) return @default;
        Variant value = m_Data[section][key];

        if (typeof(IMorphonSerializable).IsAssignableFrom(typeof(T)))
        {
            Dictionary<string, Variant> data = value.As<Dictionary<string, Variant>>();
            if (data == null) return default;

            //Check for paths and load them back
            foreach (var pair in data)
            {
                if (pair.Value.As<string>().StartsWith("res://"))
                {
                    data[pair.Key] = SafeLoadResourceFromPath<Variant>(pair.Value.As<string>());
                }
            }

            return (T)MorphonAutoSerializer.Deserialize(value.As<Dictionary<string, Variant>>());
        }
        else if (typeof(Resource).IsAssignableFrom(typeof(T)))
        {
            if (value.As<string> == null) return default;
            return SafeLoadResourceFromPath<T>(value.As<string>());
        }
        return value.As<T>();
    }
    public System.Collections.Generic.IEnumerable<T> GetListValue<T>(string section, string key, System.Collections.Generic.IEnumerable<T> @default = default) where T : IMorphonSerializable
    {
        if (!HasSectionKey(section, key)) return @default;
        Array<Dictionary<string, Variant>> data = m_Data[section][key].As<Array<Dictionary<string, Variant>>>();

        return MorphonAutoSerializer.DeserializeList(data).Cast<T>().ToList();
    }

    public bool HasSection(string section)
    {
        return m_Data.ContainsKey(section);
    }
    public bool HasSectionKey(string section, string key)
    {
        if (HasSection(section)) return m_Data[section].ContainsKey(key);
        return false;
    }

    public string[] GetSections()
    {
        return m_Data.Keys.ToArray();
    }
    public string[] GetSectionKeys(string section)
    {
        if (!HasSection(section)) return System.Array.Empty<string>();
        return m_Data[section].Keys.ToArray();
    }

    public bool Save(string path)
    {
        using FileAccess file = FileAccess.Open(path, FileAccess.ModeFlags.Write);
        if (file == null) return false;

        file.StoreString(Stringify(m_Data, "", true, true));
        return true;
    }
    public bool Load(string path)
    {
        using FileAccess file = FileAccess.Open(path, FileAccess.ModeFlags.Read);
        if (file == null) return false;

        string content = file.GetAsText();
        if (content.Trim() == "") return false;

        m_Data = ParseString(content).As<Godot.Collections.Dictionary<string, Godot.Collections.Dictionary<string, Variant>>>();

        return m_Data != null;
    }

    public void Clear()
    {
        m_Data.Clear();
    }
    public void ClearSection(string section)
    {
        if (HasSection(section))
        {
            m_Data.Remove(section);
        }
    }
    public void ClearKey(string section, string key)
    {
        if (HasSectionKey(section, key))
        {
            m_Data[section].Remove(key);
        }
    }

    private static string GetResourcePath(Resource resource)
    {
        if (resource == null) return null;
        if (!resource.ResourceLocalToScene)
        {
            return resource.ResourcePath;
        }
        return null;
    }
    private static T SafeLoadResourceFromPath<[MustBeVariant] T>(string path, T @default = default)
    {
        if (!path.StartsWith("res://")) return @default;

        Variant obj = GD.Load(path);
        return obj.As<T>();
    }

    private void NewKey(string section, string key)
    {
        if (!HasSection(section))
            m_Data.Add(section, new());
        if (!m_Data[section].ContainsKey(key))
            m_Data[section].Add(key, new());
    }
}