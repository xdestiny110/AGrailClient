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
            InvokeRepeating("loopMove", 0, 0.01f);
        }

        private float offsetX = 0.0f;
        private void loopMove()
        {
            offsetX += 0.00005f;
            bg.SetTextureOffset("_MainTex", new Vector2(offsetX, 0));
        }

    }
}
