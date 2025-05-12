using System.Text;

namespace Shared.Contracts;

public class Request : ISerializable
{
    public int Id { get; private set; }
    public string Name { get; private set; }
    public DateTime SentTime { get; set; }

    public Request() { }

    public Request(int id, string name)
    {
        Id = id;
        Name = name;
    }

    public override string ToString() => $"Request: {Name} #{Id}";

    public void Serialize(Stream stream)
    {
        byte[] bytes = BitConverter.GetBytes(Id);
        stream.Write(bytes);

        stream.WriteByte((byte)Name.Length);
        stream.Write(Encoding.ASCII.GetBytes(Name));
    }

    public void Deserialize(Stream stream)
    {
        var b1 = (byte)stream.ReadByte();
        var b2 = (byte)stream.ReadByte();
        var b3 = (byte)stream.ReadByte();
        var b4 = (byte)stream.ReadByte();
        Id = BitConverter.ToInt32([b1, b2, b3, b4], 0);

        var length = stream.ReadByte();

        var nameByteArray = new byte[length];
        stream.Read(nameByteArray, 0, length);

        Name = Encoding.ASCII.GetString(nameByteArray);
    }
}