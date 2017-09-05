using System;
using Framework.FSM;
using Framework.Message;

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
				BattleData.Instance.Agent.SelectCards.Clear();
				BattleData.Instance.Agent.SelectPlayers.Clear();
                if (t == typeof(StateIdle))
                {
                    //一般是多次行动时的取消
                    Current.Process(msg, paras);
                    BattleData.Instance.Agent.SelectSkill = null;
                    BattleData.Instance.Agent.SelectArgs.Clear();
                    MessageSystem<MessageType>.Notify(MessageType.AgentSelectPlayer);
                    MessageSystem<MessageType>.Notify(MessageType.AgentSelectCard);
                    MessageSystem<MessageType>.Notify(MessageType.AgentSelectSkill);
                }
                else
                    ChangeState(t, false, msg, paras);
            }
        }
    }
}

