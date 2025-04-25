using Godot;
using Godot.Collections;

namespace Morphon;
public interface IMorphonSerializable
{
    public void Serialize(out Dictionary<string, Variant> data);
    public void Deserialize(Dictionary<string, Variant> data);
}