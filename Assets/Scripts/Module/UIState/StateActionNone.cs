using UnityEngine;
using System.Collections;
using Framework.FSM;
using System;

namespace AGrail
{
    public class StateActionNone : UIStateBase
    {
        public StateActionNone(StateMachine<UIStateMsg> machine) : base(machine)
        {
        }

        public override string StateName
        {
            get
            {
                return "ActionNone";
            }
        }

        public override uint StateNumber
        {
            get
            {
                return 16;
            }
        }
    }
}

