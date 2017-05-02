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
        private Button btnOK;
        [SerializeField]
        private Text lblGemNum;
        [SerializeField]
        private Text lblCrystalNum;

        private uint gemNum = 0;
        private uint crystalNum = 0;
        private Func<uint, uint, bool> check = null;

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
                btnOK.onClick.RemoveAllListeners();
                btnOK.onClick.AddListener(()=> { ((Action<uint, uint>)value[0])(gemNum, crystalNum); });
                check = (Func<uint, uint, bool>)value[1];
            }
        }

        public override void Awake()
        {
            btnOK.interactable = false;
            base.Awake();
        }

        public void OnGemPlusClick()
        {
            if(gemNum < BattleData.Instance.Gem[BattleData.Instance.MainPlayer.team])
            {
                gemNum++;
                lblGemNum.text = gemNum.ToString();
            }
            btnOK.interactable = check(gemNum, crystalNum);
        }

        public void OnGemMinusClick()
        {
            if (crystalNum < BattleData.Instance.Crystal[BattleData.Instance.MainPlayer.team])
            {
                gemNum++;
                lblCrystalNum.text = crystalNum.ToString();
            }
            btnOK.interactable = check(gemNum, crystalNum);
        }

        public void OnCrystalPlusClick()
        {
            if (gemNum > 0)
            {
                gemNum--;
                lblGemNum.text = gemNum.ToString();
            }
            btnOK.interactable = check(gemNum, crystalNum);
        }

        public void OnCrystalMinusClick()
        {
            if (crystalNum >0)
            {
                crystalNum--;
                lblCrystalNum.text = crystalNum.ToString();
            }
            btnOK.interactable = check(gemNum, crystalNum);
        }
    }
}


