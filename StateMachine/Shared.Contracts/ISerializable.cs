namespace Shared.Contracts;

public interface ISerializable
{
    void Serialize(Stream stream);
    void Deserialize(Stream stream);
}