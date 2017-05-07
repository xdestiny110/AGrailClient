using Framework.FSM;

namespace AGrail
{
    public class StateArgs : UIStateBase
    {

        public override string StateName
        {
            get
            {
                return "Args";
            }
        }

        public override uint StateNumber
        {
            get
            {
                return 13;
            }
        }

        public StateArgs(StateMachine<UIStateMsg> machine) : base(machine) { }

        public override void Enter(UIStateMsg msg, params object[] paras)
        {
            BattleData.Instance.Agent.SelectArgs.Clear();
            base.Enter(msg, paras);
        }

        public override void Process(UIStateMsg msg, params object[] paras)
        {
            switch (msg)
            {
                case UIStateMsg.ClickBtn:
                    if (msg == UIStateMsg.ClickBtn && paras[0].ToString() == "OK")
                    {
                        var t = stateMachine.History.Peek();
                        stateMachine.ChangeState(t, true, msg);
                    }
                    break;
                default:
                    base.Process(msg, paras);
                    break;
            }
        }
    }
}


