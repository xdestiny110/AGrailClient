using Framework.FSM;
using Framework.Message;

namespace AGrail
{
    public class StateMagic : UIStateBase
    {
        public override string StateName
        {
            get
            {
                return "Magic";
            }
        }

        public override uint StateNumber
        {
            get
            {
                return 2;
            }
        }

        public StateMagic(StateMachine<UIStateMsg> machine) : base(machine) { }

        public override void Process(UIStateMsg msg, params object[] paras)
        {
            switch (msg)
            {
                case UIStateMsg.ClickCard:
                    if (BattleData.Instance.Agent.SelectCards.Count == 0)
                        stateMachine.BackState(UIStateMsg.Init, paras);
                    base.Process(msg, paras);
                    break;
                case UIStateMsg.ClickSkill:
                    stateMachine.ChangeState<StateSkill>(msg, true, BattleData.Instance.Agent.SelectSkill.Value, paras);
                    break;
                default:
                    base.Process(msg, paras);
                    break;
            }
        }
    }
}
