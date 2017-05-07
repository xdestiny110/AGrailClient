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

        public StateWeaken(StateMachine<UIStateMsg> machine) : base(machine) { }
    }
}


