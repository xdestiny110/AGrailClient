using Framework.UI;
using UnityEngine;
using UnityEngine.UI;

namespace AGrail
{
    public class SpecialActionUI : WindowsBase
    {
        [SerializeField]
        private Button btnBuy;
        [SerializeField]
        private Button btnExtract;
        [SerializeField]
        private Button btnSynthetize;
        [SerializeField]
        private Button btnBack;

        public override WindowType Type
        {
            get
            {
                return WindowType.SpecialActionUI;
            }
        }

        public override void Awake()
        {
            btnBuy.onClick.AddListener(onBtnBuyClick);
            btnExtract.onClick.AddListener(onBtnExtractClick);
            btnSynthetize.onClick.AddListener(onBtnSynthetizeClick);
            btnBack.onClick.AddListener(onBtnBackClick);

            base.Awake();
        }

        private void onBtnBuyClick()
        {
            onBtnBackClick();
            BattleData.Instance.Agent.FSM.HandleMessage(UIStateMsg.ClickBtn, "Buy");
        }

        private void onBtnExtractClick()
        {
            onBtnBackClick();
            BattleData.Instance.Agent.FSM.HandleMessage(UIStateMsg.ClickBtn, "Extract");
        }

        private void onBtnSynthetizeClick()
        {
            onBtnBackClick();
            BattleData.Instance.Agent.FSM.HandleMessage(UIStateMsg.ClickBtn, "Syntheis");
        }

        private void onBtnBackClick()
        {            
            GameManager.UIInstance.PopWindow(WinMsg.Resume);
        }
    }
}
