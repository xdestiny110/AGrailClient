using UnityEngine;

namespace Network
{
    public class AsyncEventQueue : MonoBehaviour
    {
        //保证Unity主线程中处理TCP回调
        void Update()
        {
            NetworkManager.Instance.Process(5);
        }
    }
}
