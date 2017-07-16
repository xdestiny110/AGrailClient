using Framework.FSM;
using Framework.Message;

namespace AGrail
{
    public class StateDropCovered : UIStateBase
    {

        public override string StateName
        {
            get
            {
                return "DropCovered";
            }
        }

        public override uint StateNumber
        {
            get
            {
                return 8;
            }
        }

        public override void Enter(UIStateMsg msg, params object[] paras)
        {
            MessageSystem<MessageType>.Notify(Framework.Message.MessageType.AgentHandChange, true);
            base.Enter(msg, paras);
        }

        public override void Exit(UIStateMsg msg, params object[] paras)
        {
            MessageSystem<MessageType>.Notify(Framework.Message.MessageType.AgentHandChange, false);
            base.Exit(msg, paras);
        }

        public StateDropCovered(StateMachine<UIStateMsg> machine) : base(machine) { }
    }
}
