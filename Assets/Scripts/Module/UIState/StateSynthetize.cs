using UnityEngine;
using System.Collections;
using Framework.FSM;

namespace AGrail
{
    public class StateSynthetize : UIStateBase
    {
        public override string StateName
        {
            get
            {
                return "Synthetize";
            }
        }

        public override uint StateNumber
        {
            get
            {
                return 14;
            }
        }

        public StateSynthetize(StateMachine<UIStateMsg> machine) : base(machine) { }
    }
}
