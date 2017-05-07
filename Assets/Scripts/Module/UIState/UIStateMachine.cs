using System;
using Framework.FSM;

namespace AGrail
{
    public class UIStateMachine : StateMachine<UIStateMsg>
    {
        public UIStateMachine(string name) : base(name)
        {

        }

        protected override bool cleanHistroy(Type t)
        {
            if (t == typeof(StateIdle))
                return true;
            return false;
        }
    }
}

