using Framework.FSM;

namespace AGrail
{
    public class StateWeaken : UIStateBase
    {

        public override string StateName
        {
            get
            {
                return "Weaken";
            }
        }

        public override uint StateNumber
        {
            get
            {
                return 6;
            }
        }

        public StateWeaken(StateMachine<StateMsg> machine) : base(machine) { }

        public override void Process(StateMsg msg, params object[] paras)
        {

        }
    }
}


