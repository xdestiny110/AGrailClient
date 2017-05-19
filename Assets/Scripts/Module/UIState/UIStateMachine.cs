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

        public override void BackState(UIStateMsg msg, params object[] paras)
        {
            if (History.Count > 0)
            {                
                var t = History.Pop();
                if (t == typeof(StateIdle))
                    Current.Process(msg, paras);
                else
                    ChangeState(t, false, msg, paras);
            }
        }
    }
}

