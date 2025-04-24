using System.Collections.Generic;
using System.Linq;
using Godot;

namespace Morphon;
public partial class MorphonConfigFile : Json
{
    private Godot.Collections.Dictionary<string, Godot.Collections.Dictionary<string, Variant>> m_Data = new();

    /// <summary>
    /// Set a Variant value (It can only be a type that is supported by Json)
    /// Resources that are not local to scene will be saved by their path
    /// </summary>
    public void SetValue(string section, string key, Variant value)
    {
        if (!HasSection(section))
            m_Data.Add(section, new());
        if (!m_Data[section].ContainsKey(key))
            m_Data[section].Add(key, new());

        Resource rValue = value.As<Resource>();

        if (rValue != null)
        {
            m_Data[section][key] = GetResourcePath(rValue);
            return;
        }

        m_Data[section][key] = value;
    }
    public void SetValue(string section, string key, IMorphonSerializable value)
    {
        if (!HasSection(section))
            m_Data.Add(section, new());
        if (!m_Data[section].ContainsKey(key))
            m_Data[section].Add(key, new());

        m_Data[section][key] = value.Serialize();
    }
    public void SetValue(string section, string key, IEnumerable<IMorphonSerializable> list)
    {
        if (!HasSection(section))
            m_Data.Add(section, new());
        if (!m_Data[section].ContainsKey(key))
            m_Data[section].Add(key, new());

        m_Data[section][key] = MorphonAutoSerializer.SerializeList(list.ToList());
    }

    public T GetValue<[MustBeVariant] T>(string section, string key, T @default = default)
    {
        if (!HasSectionKey(section, key)) return @default;
        Variant value = m_Data[section][key];

        if (typeof(IMorphonSerializable).IsAssignableFrom(typeof(T)))
        {
            return (T)MorphonAutoSerializer.Deserialize(value.As<string>());
        }
        else if (typeof(Resource).IsAssignableFrom(typeof(T)))
        {
            return SafeLoadResourceFromPath<T>(value.As<string>());
        }

        return value.As<T>();
    }
    public List<T> GetListValue<T>(string section, string key, List<T> @default = default) where T : IMorphonSerializable
    {
        if (!HasSectionKey(section, key)) return @default;
        string jsonData = m_Data[section][key].As<string>();

        return MorphonAutoSerializer.DeserializeList(jsonData).Cast<T>().ToList();
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