using UnityEngine;
using System.Collections;
namespace Common
{
    public interface IPoolUser 
    {
        void OnPoolInstanceDone(string userBundleName, object obj);
    }
}