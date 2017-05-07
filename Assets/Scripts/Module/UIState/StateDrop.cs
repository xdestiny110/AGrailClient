using Framework.FSM;

namespace AGrail
{
    public class StateDrop : UIStateBase
    {

        public override string StateName
        {
            get
            {
                return "Drop";
            }
        }

        public override uint StateNumber
        {
            get
            {
                return 5;
            }
        }

        public StateDrop(StateMachine<UIStateMsg> machine) : base(machine) { }
    }
}


