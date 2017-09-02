using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace AGrail
{
    public class LobbyTools : MonoBehaviour
    {
        [SerializeField]
        private Button btnSetting;

        void Awake()
        {
            btnSetting.onClick.AddListener(onBtnSettingClick);
        }

        private void onBtnSettingClick()
        {
            if (GameManager.UIInstance.PeekWindow() == Framework.UI.WindowType.OptionsUI)
                GameManager.UIInstance.PopWindow(Framework.UI.WinMsg.None);
            else
                GameManager.UIInstance.PushWindow(Framework.UI.WindowType.OptionsUI, Framework.UI.WinMsg.None, 59);
        }
    }
}
