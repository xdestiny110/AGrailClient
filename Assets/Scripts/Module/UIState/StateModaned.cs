using UnityEngine;
using System.Collections;
using Framework.FSM;

namespace AGrail
{
    public class StateModaned : StateBase<StateMsg>
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

        public StateModaned(StateMachine<StateMsg> machine) : base(machine) { }

        public override void Process(StateMsg msg, params object[] paras)
        {
            
        }
    }
}


