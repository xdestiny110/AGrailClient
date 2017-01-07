using System;
using Framework.Network;
using ProtoBuf;
using System.IO;

namespace AGrail
{
    public class Coder : ICoder
    {
        private MemoryStream stream = new MemoryStream();        

        public bool Decode(byte[] data, out IExtensible proto)
        {
            try
            {
                // 将新接收的数据写入stream尾
                stream.Position = stream.Length;
                stream.Write(data, 0, data.Length);
                stream.Position = 0;
                // 4个字节协议长度
                
                var sumLen = BitConverter.ToInt32(data, pos);

                //var pos = 0;

                //pos = 6;
                //// 2个字节协议种类
                //var protoID = BitConverter.ToInt16(data, pos);
                //pos = 8;
                //MemoryStream stream = new MemoryStream();
                //stream.Write(data, pos, sumLen - 4);
                //stream.Position = 0;
                //proto = (IExtensible)ProtoSerializer.ParseFrom((ProtoNameIds)protoID, stream);
            }
            catch (Exception ex)
            {
                UnityEngine.Debug.LogError("Decode protobuf fail!");
                UnityEngine.Debug.LogException(ex);
                proto = null;
                return false;
            }
            return true;
        }

        public byte[] Encode(IExtensible proto)
        {
            
        }
    }
}


