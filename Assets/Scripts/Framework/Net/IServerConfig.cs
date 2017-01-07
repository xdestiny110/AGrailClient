using UnityEngine;
using System.Collections;

namespace Framework.Network
{
    public interface IServerConfig
    {
        string ServerIP { get; }
        int Port { get; }
    }
}
