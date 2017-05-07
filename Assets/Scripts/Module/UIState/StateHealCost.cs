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

        public StateHealCost(StateMachine<StateMsg> machine) : base(machine) { }

        public override void Process(StateMsg msg, params object[] paras)
        {

        }
    }
}


