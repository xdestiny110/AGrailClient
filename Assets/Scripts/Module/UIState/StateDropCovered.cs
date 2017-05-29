using Framework.FSM;

namespace AGrail
{
    public class StateDropCovered : UIStateBase
    {

        public override string StateName
        {
            get
            {
                return "DropCovered";
            }
        }

        public override uint StateNumber
        {
            get
            {
                return 8;
            }
        }

        public StateDropCovered(StateMachine<UIStateMsg> machine) : base(machine) { }
    }
}
