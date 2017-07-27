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

            base.Awake();
        }
    }
}


