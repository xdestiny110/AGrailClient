using UnityEngine;
using System.Collections;
using Framework.FSM;

namespace AGrail
{
    public class StateModaned : StateBase<UIStateMsg>
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

        public override void Process(UIStateMsg msg, params object[] paras)
        {
            
        }
    }
}


