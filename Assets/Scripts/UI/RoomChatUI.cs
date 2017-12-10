using Framework.UI;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Framework.Message;
using Framework.AssetBundle;
using System.Linq;
using DG.Tweening;
using UnityEngine.SceneManagement;

namespace AGrail
{
    public class RoomChatUI : MonoBehaviour
    {
        [SerializeField]
        private Button btnSend;
        [SerializeField]
        private InputField inputField;
        [SerializeField]
        private Text Talks;

        public string talks
        {
            set
            {
                Talks.text = value;
            }
            get
            {
                return Talks.text;
            }
        }


        void Awake()
        {
            inputField.onEndEdit.AddListener(onBtnSendClick);
            btnSend.onClick.AddListener(delegate { onBtnSendClick(null); });
        }

        private void onBtnSendClick(string str)
        {
            if (!string.IsNullOrEmpty(inputField.text))
                Dialog.Instance.SendTalk(inputField.text);
            inputField.text = string.Empty;
        }
    }
}
