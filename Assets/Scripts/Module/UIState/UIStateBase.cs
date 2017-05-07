using Framework.FSM;
using Framework.Message;

namespace AGrail
{
    public abstract class UIStateBase : StateBase<StateMsg>
    {
        public UIStateBase(StateMachine<StateMsg> machine) : base(machine) { }

        public override void Enter(StateMsg msg, params object[] paras)
        {            
            MessageSystem<MessageType>.Notify(MessageType.AgentUIStateChange);
            base.Enter(msg, paras);
        }
    }
}

