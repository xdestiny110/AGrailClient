using ProtoBuf;

namespace Framework.Network
{
    public interface ICoder
    {
        bool Decode(byte[] data, out IExtensible proto);
        byte[] Encode(IExtensible proto);
    }
}


