using Framework.FSM;

namespace AGrail
{
    public class StateHealCost : UIStateBase
    {

        public override string StateName
        {
            get
            {
                return "HealCost";
            }
        }

        public override uint StateNumber
        {
            get
            {
                return 7;
            }
        }

        public StateHealCost(StateMachine<UIStateMsg> machine) : base(machine) { }

        public override void Process(UIStateMsg msg, params object[] paras)
        {

        }
    }
}


