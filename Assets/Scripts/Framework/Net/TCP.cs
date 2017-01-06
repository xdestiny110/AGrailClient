using System;
using System.Net.Sockets;
using System.Diagnostics;

namespace Framework.Network
{
    public sealed class TCP
    {
        private Socket socket;
        private bool startReconnect;
        private int _reconnectDelay;
        private int _reconnectDelayMin = 1000;
        private int _reconnectDelayMax = 60000;
        private bool _autoReconnect;

        private const int inputSize = 4096;
        private const int reserveInputBufSize = 8192;
        private const int reserveOutputBufSize = 1024;

        private readonly ConcurrentQueue<Action> actions = new ConcurrentQueue<Action>();
        private readonly Stopwatch reconnectWatcher = new Stopwatch();
        private readonly Stopwatch frameWatcher = new Stopwatch();


        public bool Connected { get { return null != socket && socket.Connected; } }

        public TCP()
        {

        }



        //public void Connect(string )

    }
}


