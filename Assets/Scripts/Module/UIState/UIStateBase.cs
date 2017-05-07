using System;
using Framework.FSM;
using Framework.Message;
using UnityEngine;

namespace AGrail
{
    public abstract class UIStateBase : StateBase<UIStateMsg>
    {
        public UIStateBase(StateMachine<UIStateMsg> machine) : base(machine) { }

        public override void Enter(UIStateMsg msg, params object[] paras)
        {            
            MessageSystem<MessageType>.Notify(MessageType.AgentUIStateChange);
            base.Enter(msg, paras);
        }

        public override void Process(UIStateMsg msg, params object[] paras)
        {
            MessageSystem<MessageType>.Notify(MessageType.AgentUIStateChange);
            Debug.LogFormat("Process {0}:{1}", StateName, StateNumber);
        }
    }
}

