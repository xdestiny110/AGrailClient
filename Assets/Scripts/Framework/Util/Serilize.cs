using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

namespace Framework
{
    public static class Serilizer<T>
    {
        public static string Serialize(T data)
        {
            if (data == null)
                throw new ArgumentNullException();
            var formatter = new BinaryFormatter();
            var stream = new MemoryStream();
            formatter.Serialize(stream, data);
            stream.Position = 0;
            var buffer = new byte[stream.Length];
            stream.Read(buffer, 0, buffer.Length);
            stream.Flush();
            stream.Close();
            return Convert.ToBase64String(buffer);
        }

        public static T Desrialize(string str)
        {
            if (string.IsNullOrEmpty(str))
                throw new ArgumentNullException();
            var ret = default(T);
            var formatter = new BinaryFormatter();
            var buffer = Convert.FromBase64String(str);
            var stream = new MemoryStream(buffer);
            ret = (T)formatter.Deserialize(stream);
            stream.Flush();
            stream.Close();
            return ret;
        }
    }
}


