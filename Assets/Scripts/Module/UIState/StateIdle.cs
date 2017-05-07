using UnityEngine;
using System.Collections;
using Framework.FSM;
using System;

namespace AGrail
{
    public class StateIdle : StateBase<StateMsg>
    {
        public override string StateName
        {
            get
            {
                return "Idle";
            }
        }

        public StateIdle(StateMachine<StateMsg> machine) : base(machine) { }

        public override void Process(StateMsg msg, params object[] paras)
        {
            
        }
    }
}