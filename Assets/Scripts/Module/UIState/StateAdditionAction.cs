using Framework.FSM;
using System;

namespace AGrail
{
    public class StateAdditionAction : UIStateBase
    {
        public StateAdditionAction(StateMachine<UIStateMsg> machine) : base(machine)
        {
        }

        public override string StateName
        {
            get
            {
                return "AdditionAction";
            }
        }

        public override uint StateNumber
        {
            get
            {
                return 15;
            }
        }
    }
}
