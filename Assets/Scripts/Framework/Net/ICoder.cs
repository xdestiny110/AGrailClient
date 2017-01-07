using ProtoBuf;

namespace Framework.Network
{
    public interface ICoder
    {
        bool Decode(byte[] stream, out IExtensible proto);
        byte[] Encode(IExtensible proto);
    }
}


