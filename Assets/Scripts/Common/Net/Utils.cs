using UnityEngine;
using network;
using System;
using System.IO;
using System.Collections.Generic;

namespace Network
{
    public static class Utils
    {
        public static int ReadBytes(Stream stream, int length)
        {
            //2016/6/9
            //By Xdestiny:
            //根据抓包的结果，包前面表征数据长度的数据似乎是以小端方式存储的
            //其实最好是使用Bitconver来进行转换，没必要自己写
            int n = 0;
            for (int i = 0; i < length; ++i)
                //n = n * 256 + stream.ReadByte();
                n += stream.ReadByte() * (int)Mathf.Pow(256, i);
            return n;
        }

        /// <summary>
        /// 将序列化生成的stream转化为实际发送的stream
        /// <para>整个数据流的结构为</para>
        /// <para>4个字节的包长度(不算在长度中), 2个字节的包长度, 2个字节的请求类型</para>
        /// <para>By Xdestiny 2016/6/10</para>
        /// </summary>
        /// <param name="serializeStream">序列化生成的stream</param>
        /// <returns>实际发送的stream</returns>
        public static MemoryStream SerializeStreamToSendStream(MemoryStream serializeStream, ProtoNameIds type)
        {
            MemoryStream sendStream = new MemoryStream();
            sendStream.SetLength(serializeStream.Length + 8);
            sendStream.Position = 0;
            sendStream.Write(BitConverter.GetBytes((int)sendStream.Length - 4), 0, 4);
            sendStream.Write(BitConverter.GetBytes((short)(sendStream.Length - 4)), 0, 2);
            sendStream.Write(BitConverter.GetBytes((short)type), 0, 2);
            sendStream.Write(serializeStream.GetBuffer(), 0, (int)serializeStream.Length);
            return sendStream;
        }

        public static void SerializeAndSend<T>(T @object, ProtoNameIds id)
        {
            MemoryStream serializeStream = new MemoryStream();
            ProtoSerializer.Serialize(id, serializeStream, @object);
            MemoryStream sendStream = SerializeStreamToSendStream(serializeStream, id);
            NetworkManager.Instance.Send(sendStream);
        }
    }
}

