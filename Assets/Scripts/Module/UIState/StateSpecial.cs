using Framework.FSM;

namespace AGrail
{
    public class StateSpecial : UIStateBase
    {
        public override string StateName
        {
            get
            {
                return "Special";
            }
        }

        public override uint StateNumber
        {
            get
            {
                return 12;
            }
        }

        public StateSpecial(StateMachine<UIStateMsg> machine) : base(machine) { }

        public override void Process(UIStateMsg msg, params object[] paras)
        {
            
        }
    }
}
