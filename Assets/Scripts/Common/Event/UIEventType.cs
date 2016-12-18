using UnityEngine;
using System.Collections;
namespace EventSystem
{
    public enum EventType
    {
        Null = 0,
        SocketResponse, //parm1: true代表成功，false代表失败
    }
}