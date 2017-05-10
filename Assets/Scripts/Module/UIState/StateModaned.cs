using UnityEngine;
using System.Collections;
using Framework.FSM;
using System;

namespace AGrail
{
    public class StateModaned : UIStateBase
    {

        public override string StateName
        {
            get
            {
                return "Modaned";
            }
        }

        public override uint StateNumber
        {
            get
            {
                return 4;
            }
        }

        public StateModaned(StateMachine<UIStateMsg> machine) : base(machine) { }

    }
}


