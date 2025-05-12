using System.Text;

namespace Shared.Contracts;

public class Response : ISerializable
{
    public int? RequestId { get; private set; }
    public string Name { get; private set; }

    public Response() { }
    
    public Response(int? requestId, string name)
    {
        RequestId = requestId;
        Name = name;
    }

    public override string ToString() => $"Response: {Name} #{RequestId}";

    public void Serialize(Stream stream)
    {
        unsafe
        {
            bool input = RequestId.HasValue;
            byte value = *((byte*)(&input)); // 0 / 1
            stream.WriteByte(value);
        }

        if (RequestId.HasValue)
        {
            byte[] bytes = BitConverter.GetBytes(RequestId.Value);
            stream.Write(bytes);
        }

        stream.WriteByte((byte)Name.Length);
        stream.Write(Encoding.ASCII.GetBytes(Name));
    }

    public void Deserialize(Stream stream)
    {
        var hasRequestId = stream.ReadByte() == 1;
        if (hasRequestId)
        {
            RequestId = BitConverter.ToInt32([(byte)stream.ReadByte(), (byte)stream.ReadByte(), (byte)stream.ReadByte(), (byte)stream.ReadByte()], 0);
        }
        
        var length = stream.ReadByte();
        
        var nameByteArray = new byte[length];
        stream.Read(nameByteArray, 0, length);

        Name = Encoding.ASCII.GetString(nameByteArray);
    }
}