using UnityEngine;
using System.Collections;
using UnityEngine.UI;

namespace AGrail
{
    [RequireComponent(typeof(Image))]
    public class BGLoopMove : MonoBehaviour
    {
        private Material bg;

        void Awake()
        {
            bg = GetComponent<Image>().material;
        }

        void Update()
        {
            //bg.SetTextureOffset
        }
    }
}
