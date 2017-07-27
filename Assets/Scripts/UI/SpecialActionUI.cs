using Framework.UI;

namespace AGrail
{
    public class SpecialActionUI : WindowsBase
    {
        public override WindowType Type
        {
            get
            {
                return WindowType.SpecialActionUI;
            }
        }

        public void OnBtnBuyClick()
        {
            BattleData.Instance.Agent.FSM.HandleMessage(UIStateMsg.ClickBtn, "Buy");
        }

        public void OnBtnExtractClick()
        {
            BattleData.Instance.Agent.FSM.HandleMessage(UIStateMsg.ClickBtn, "Extract");
        }

        public void OnBtnSynthetizeClick()
        {
            BattleData.Instance.Agent.FSM.HandleMessage(UIStateMsg.ClickBtn, "Syntheis");
        }
    }
}
