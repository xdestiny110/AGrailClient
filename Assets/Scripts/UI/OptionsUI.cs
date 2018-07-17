using Framework.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace AGrail
{
    public class OptionsUI : UIBase
    {
        [SerializeField]
        private Transform root;
        [SerializeField]
        private Slider bgmSlider;
        [SerializeField]
        private Slider seSlider;

        public override string Type
        {
            get
            {
                return WindowType.OptionsUI.ToString();
            }
        }

        public override void Awake()
        {
            bgmSlider.value = AudioManager.Instance.BGMVolume;
            seSlider.value = AudioManager.Instance.SEVolume;

            base.Awake();
        }

        public void OnExitClick()
        {
            GameManager.UIInstance.PopWindow(WinMsg.None);
            if (GameManager.UIInstance.PeekWindowType() == WindowType.LoginBox.ToString() ||
                GameManager.UIInstance.PeekWindowType() == WindowType.Lobby.ToString())
                return;
            else
            {
                Lobby.Instance.LeaveRoom();
                if(SceneManager.GetActiveScene().buildIndex==2)
                SceneManager.LoadScene(1);
                else
                GameManager.UIInstance.PopAllWindow();
                GameManager.UIInstance.PushWindow(WindowType.Lobby, WinMsg.None);
            }
        }

        public void OnBGMVolChange(float vol)
        {
            AudioManager.Instance.BGMVolume = vol;
        }

        public void OnSEVolChange(float vol)
        {
            AudioManager.Instance.SEVolume = vol;
        }

        public void OnBackClick()
        {
            GameManager.UIInstance.PopWindow(WinMsg.None);
        }
    }
}
