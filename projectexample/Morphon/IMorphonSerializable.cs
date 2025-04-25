using Godot;
using Godot.Collections;

namespace Morphon;
public interface IMorphonSerializable
{
    public Dictionary<string, Variant> Serialize();
    public void Deserialize(Dictionary<string, Variant> data);
}