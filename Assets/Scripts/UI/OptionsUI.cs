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
        [SerializeField]
        private Slider uiSlider;
        [SerializeField]
        private Button resetButton;

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
            uiSlider.value = GameManager.UIInstance.UIEdge;

            bgmSlider.onValueChanged.AddListener(OnBGMVolChange);
            seSlider.onValueChanged.AddListener(OnSEVolChange);
            uiSlider.onValueChanged.AddListener(OnUIEdgeChange);
            resetButton.onClick.AddListener(OnReset);

            base.Awake();
        }

        private void OnReset()
        {
            //OnUIEdgeChange(.5f);
            uiSlider.value = .5f;
        }

        public  override void OnDestroy()
        {
            bgmSlider.onValueChanged.RemoveAllListeners();
            seSlider.onValueChanged.RemoveAllListeners();
            uiSlider.onValueChanged.RemoveAllListeners();
            resetButton.onClick.RemoveAllListeners();

            base.OnDestroy();
        }

        public void OnExitClick()
        {
            GameManager.UIInstance.PopWindow(WinMsg.None);
            if (GameManager.UIInstance.PeekWindowType() == WindowType.LoginBox ||
                    GameManager.UIInstance.PeekWindowType() == WindowType.Lobby)
                return;
            else
            {
                Lobby.Instance.LeaveRoom();
                if (SceneManager.GetActiveScene().buildIndex == 2)
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

        public void OnUIEdgeChange(float vol)
        {
            GameManager.UIInstance.UIEdge = vol;
            var fixList = FindObjectsOfType<UIEdgeFix>();
            foreach (var item in fixList)
            {
                item.FixEdge(vol);
            }
        }
    }
}
