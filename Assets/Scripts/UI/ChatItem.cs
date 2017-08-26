using Framework.AssetBundle;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace AGrail
{
    public class ChatItem : MonoBehaviour
    {
        [SerializeField]
        private Image role;
        [SerializeField]
        private Text msg;

        public uint? RoleID
        {
            set
            {
                if(value != null)
                {
                    var sprite = AssetBundleManager.Instance.LoadAsset<Sprite>("hero_s", value.ToString() + "S");
                    if (sprite != null)
                        role.sprite = sprite;
                }
            }
        }

        public string Msg
        {
            set
            {
                msg.text = value;
            }
        }

        public bool IsMainPlayer
        {
            set
            {
                if (!value)
                    role.transform.SetAsLastSibling();
            }
        }
    }
}
