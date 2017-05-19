using Framework.FSM;

namespace AGrail
{
    public class StateExtract : UIStateBase
    {

        public override string StateName
        {
            get
            {
                return "Extract";
            }
        }

        public override uint StateNumber
        {
            get
            {
                return 13;
            }
        }

        public StateExtract(StateMachine<UIStateMsg> machine) : base(machine) { }
    }
}


