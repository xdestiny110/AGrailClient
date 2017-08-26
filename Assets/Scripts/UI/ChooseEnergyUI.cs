using UnityEngine;
using System.Collections;
using Framework.UI;
using System;
using UnityEngine.UI;

namespace AGrail
{
    public class ChooseEnergyUI : WindowsBase
    {
        [SerializeField]
        private Transform root;
        [SerializeField]
        private Button btn2Crystal;
        [SerializeField]
        private Button btn1Crystal;
        [SerializeField]
        private Button btn1Crystal1Gem;
        [SerializeField]
        private Button btn1Gem;
        [SerializeField]
        private Button btn2Gem;
        [SerializeField]
        private Button btnBack;

        public override WindowType Type
        {
            get
            {
                return WindowType.ChooseEnergy;
            }
        }

        public override object[] Parameters
        {
            get
            {
                return base.Parameters;
            }

            set
            {
                base.Parameters = value;
            }
        }

        public override void Awake()
        {
            btn1Crystal.GetComponent<Image>().alphaHitTestMinimumThreshold = 0.8f;
            btn2Crystal.GetComponent<Image>().alphaHitTestMinimumThreshold = 0.8f;
            btn1Crystal1Gem.GetComponent<Image>().alphaHitTestMinimumThreshold = 0.8f;
            btn1Gem.GetComponent<Image>().alphaHitTestMinimumThreshold = 0.8f;
            btn2Gem.GetComponent<Image>().alphaHitTestMinimumThreshold = 0.8f;

            btnBack.onClick.AddListener(onBtnBackClick);
            btn1Gem.onClick.AddListener(delegate { onBtnEnergyClick(0); } );
            btn2Gem.onClick.AddListener(delegate { onBtnEnergyClick(1); });
            btn1Crystal1Gem.onClick.AddListener(delegate { onBtnEnergyClick(2); });
            btn1Crystal.onClick.AddListener(delegate { onBtnEnergyClick(3); });
            btn2Crystal.onClick.AddListener(delegate { onBtnEnergyClick(4); });

            base.Awake();
        }

        private void onBtnBackClick()
        {
            GameManager.UIInstance.PopWindow(WinMsg.Resume);
        }

        private void onBtnEnergyClick(int idx)
        {
            switch (idx)
            {
                case 0:
                    break;
                case 1:
                    break;
                case 2:
                    break;
                case 3:
                    break;
                case 4:
                    break;
            }
        }
    }
}


