using UnityEngine;
using System.Collections;
using Framework.Network;

namespace Framework
{
    public class GameManager : MonoBehaviour
    {        
        public static TCP TCPInstance { get; private set; }

        void Awake()
        {
            //TCPInstance = new TCP()
        }

        void Update()
        {
            TCPInstance.DoActions();
        }
    }
}


