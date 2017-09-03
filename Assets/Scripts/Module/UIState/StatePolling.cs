using Framework.FSM;
using System;

namespace AGrail
{
    public class StatePolling : UIStateBase
    {
        public StatePolling(StateMachine<UIStateMsg> machine) : base(machine)
        {
        }

        public override string StateName
        {
            get
            {
                return "Polling";
            }
        }

        public override uint StateNumber
        {
            get
            {
                return 999;
            }
        }
    }
}

