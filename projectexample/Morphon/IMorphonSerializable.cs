namespace Morphon;
public interface IMorphonSerializable
{
    public string Serialize();
    public void Deserialize(string jsonData);
}