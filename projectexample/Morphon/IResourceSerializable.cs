public interface IResourceSerializable
{
    public string Serialize();
    public void Deserialize(string jsonData);
}