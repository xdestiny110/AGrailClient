using System;
using Framework.Network;
using ProtoBuf;
using System.IO;
using System.Collections.Generic;

namespace AGrail
{
    public class Coder : ICoder
    {
        private MemoryStream stream = new MemoryStream();
        private BinaryReader br;

        public Coder()
        {
            br = new BinaryReader(stream);
        }

        public bool Decode(byte[] data, out List<Protobuf> protobufs)
        {
            protobufs = new List<Protobuf>();
            try
            {
                // 将新接收的数据写入stream尾
                stream.Position = stream.Length;
                stream.Write(data, 0, data.Length);
                stream.Position = 0;
                // 4个字节协议长度
                var sumLen = br.ReadInt32();
                while(sumLen + 4 <= stream.Length)
                {
                    // 2个字节协议号
                    stream.Position = 6;
                    var protoID = br.ReadInt16();
                    // 协议反序列化
                    MemoryStream protoStream = new MemoryStream();
                    protoStream.Write(stream.ToArray(), 8, sumLen - 4);

                    printProtoContent((ProtoNameIds)protoID, protoStream.GetBuffer(), sumLen - 4);

                    // 将流位置重置为开头
                    protoStream.Position = 0;
                    protobufs.Add(new Protobuf()
                    {
                        Proto = (IExtensible)ProtoSerializer.ParseFrom((ProtoNameIds)protoID, protoStream),
                        ProtoID = (ProtoNameIds)protoID
                    });

                    // 判断是否有粘包
                    stream.Position = sumLen + 4;
                    if (stream.Length - stream.Position > 0)
                    {
                        MemoryStream newStream = new MemoryStream();
                        newStream.Write(stream.ToArray(), (int)stream.Position, (int)(stream.Length - stream.Position));
                        stream = newStream;
                        br = new BinaryReader(stream);
                        // 判断剩下的是否包含一个完整的协议
                        stream.Position = 0;
                        sumLen = br.ReadInt32();
                    }
                    else
                    {
                        // 说明说是一个完整的包
                        stream = new MemoryStream();
                        br = new BinaryReader(stream);
                        break;
                    }
                }
            }
            catch (Exception ex)
            {
                UnityEngine.Debug.LogError("Decode protobuf fail!");
                UnityEngine.Debug.LogException(ex);
                return false;
            }
            return true;
        }

        public byte[] Encode(Protobuf protobuf)
        {
            try
            {
                MemoryStream serializeStream = new MemoryStream();
                ProtoSerializer.Serialize(protobuf.ProtoID, serializeStream, protobuf.Proto);
                // 整个数据流结构为
                // 4个字节的包长度(不算在长度中), 2个字节的包长度, 2个字节的请求类型
                MemoryStream sendStream = new MemoryStream();
                sendStream.SetLength(serializeStream.Length + 8);
                sendStream.Position = 0;
                sendStream.Write(BitConverter.GetBytes((int)serializeStream.Length + 4), 0, 4);
                sendStream.Write(BitConverter.GetBytes((short)serializeStream.Length + 4), 0, 2);
                sendStream.Write(BitConverter.GetBytes((short)protobuf.ProtoID), 0, 2);
                sendStream.Write(serializeStream.GetBuffer(), 0, (int)serializeStream.Length);
                var res = new byte[sendStream.Length];
                Array.Copy(sendStream.GetBuffer(), res, sendStream.Length);
                return res;
            }
            catch(Exception ex)
            {
                UnityEngine.Debug.LogError("Encode protobuf fail!");
                UnityEngine.Debug.LogException(ex);
            }
            return null;
        }

        [System.Diagnostics.Conditional("LOGON")]
        private void printProtoContent(ProtoNameIds protoID, byte[] content, int len)
        {
            // 打印协议内容
            System.Text.StringBuilder log = new System.Text.StringBuilder();
            for (int i = 0; i < len; i++)
            {
                log.Append(content[i]);
                log.Append(", ");
            }
            UnityEngine.Debug.LogFormat("{0}: {1}", protoID, log);
        }

    }
}


