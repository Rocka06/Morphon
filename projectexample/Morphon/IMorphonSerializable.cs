using Godot;
using Godot.Collections;

namespace Morphon;
public interface IMorphonSerializable
{
    /// <summary>
    /// A function to serialize a class into types that can be handled by json
    /// If you add a Resource to the dictionary that is not local to scene, then it's path will be saved, then loaded back later
    /// </summary>
    /// <param name="data"></param>
    public void Serialize(out Dictionary<string, Variant> data);
    public void Deserialize(Dictionary<string, Variant> data);
}