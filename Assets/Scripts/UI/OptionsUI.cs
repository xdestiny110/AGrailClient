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
        private Slider volSlider;

        public override WindowType Type
        {
            get
            {
                return WindowType.OptionsUI;
            }
        }

        public override void Awake()
        {
            volSlider.value = AudioListener.volume;

            base.Awake();
        }

        public void OnExitClick()
        {
            Lobby.Instance.LeaveRoom();
            SceneManager.LoadScene(1);
        }
        
        public void OnVolChange(float vol)
        {
            AudioListener.volume = vol;
        }

        public void OnBackClick()
        {
            GameManager.UIInstance.PopWindow(WinMsg.None);
        }
    }
}
