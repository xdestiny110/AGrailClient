using UnityEngine;
using System.Collections;
using System.Runtime.InteropServices;
using System;

namespace Framework.Network
{
    public class WebSocket : SocketBase
    {
        public override bool Connected
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public override bool AutoReconnect
        {
            get
            {
                throw new NotImplementedException();
            }

            set
            {
                throw new NotImplementedException();
            }
        }

        public WebSocket(IServerConfig serverConfig, ICoder coder) : base(serverConfig, coder) { }

        public override void Connect()
        {
            ConnectJS(string.Format("ws://{0}:{1}", serverConfig.ServerIP, serverConfig.Port));
        }

        public override void Send(Protobuf protobuf)
        {
            beforeSendProto(protobuf);
            var data = coder.Encode(protobuf);
            SendJS(data, data.Length);
        }

        public override void Close()
        {
            CloseJS();
            actions.clear();
            Debug.Log("Disconnect.");
        }

        [DllImport("__Internal")]
        private static extern void ConnectJS(string str);
        [DllImport("__Internal")]
        private static extern void SendJS(byte[] data, int length);
        [DllImport("__Internal")]
        private static extern void CloseJS();
        [DllImport("__Internal")]
        private static extern void AlertJS(string str);
    }
}
