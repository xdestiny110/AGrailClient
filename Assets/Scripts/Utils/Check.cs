using UnityEngine;
using System.Collections;
namespace Util
{
    public class Check
    {
        public static bool NULL(object o)
        {
            if(o == null)
            {
                Debug.LogError("Get Null Object");
            }
            return o == null;
        }
       
    }
}