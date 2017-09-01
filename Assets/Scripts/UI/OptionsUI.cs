using Framework.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace AGrail
{
    public class OptionsUI : WindowsBase
    {
        [SerializeField]
        private Transform root;
        [SerializeField]
        private Slider bgmSlider;
        [SerializeField]
        private Slider seSlider;

        public override WindowType Type
        {
            get
            {
                return WindowType.OptionsUI;
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
            if(SceneManager.GetActiveScene().buildIndex == 2)
            {
                Lobby.Instance.LeaveRoom();
                SceneManager.LoadScene(1);
            }
            else
            {
                GameManager.UIInstance.PopWindow(WinMsg.None);
                if (GameManager.UIInstance.PeekWindow() == WindowType.LoginBox ||
                    GameManager.UIInstance.PeekWindow() == WindowType.Lobby)
                    return;                
                else
                {
                    GameManager.UIInstance.PopAllWindow();
                    GameManager.UIInstance.PushWindow(WindowType.Lobby, WinMsg.None);
                }
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
