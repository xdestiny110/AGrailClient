using ProtoBuf;
using System.Collections.Generic;

namespace Framework.Network
{
    public interface ICoder
    {
        bool Decode(byte[] data, out List<Protobuf> protobufs);
        byte[] Encode(Protobuf protobuf);
    }

    public class Protobuf
    {
        public IExtensible Proto;
        public ProtoNameIds ProtoID;
    }
}


