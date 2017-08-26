using Framework.FSM;
using Framework.Message;

namespace AGrail
{
    public class StateSkill : UIStateBase
    {
        public override string StateName
        {
            get
            {
                return "Skill";
            }
        }

        private uint skillID = 0;
        public override uint StateNumber
        {
            get
            {
                return skillID;
            }
        }

        public StateSkill(StateMachine<UIStateMsg> machine) : base(machine) { }

        public override void Enter(UIStateMsg msg, params object[] paras)
        {
            if(msg == UIStateMsg.ClickSkill || msg == UIStateMsg.Init)
            {
                if (msg == UIStateMsg.ClickSkill)
                    skillID = BattleData.Instance.Agent.SelectSkill.Value;
                else
                {
                    skillID = BattleData.Instance.Agent.Cmd.respond_id;
                    BattleData.Instance.Agent.SelectSkill = skillID;
                }
                BattleData.Instance.Agent.SelectCards.Clear();
                BattleData.Instance.Agent.SelectPlayers.Clear();
                BattleData.Instance.Agent.SelectArgs.Clear();
                MessageSystem<MessageType>.Notify(MessageType.AgentSelectPlayer);
                MessageSystem<MessageType>.Notify(MessageType.AgentSelectCard);
                MessageSystem<MessageType>.Notify(MessageType.CloseNewArgsUI);
            }
            base.Enter(msg, paras);
        }

        public override void Process(UIStateMsg msg, params object[] paras)
        {
            switch(msg)
            {
                case UIStateMsg.ClickSkill:
                    if (BattleData.Instance.Agent.SelectSkill == null)
                    {
                        //取消技能
                        stateMachine.BackState(msg, paras);
                        BattleData.Instance.Agent.SelectCards.Clear();
                        BattleData.Instance.Agent.SelectPlayers.Clear();
                        BattleData.Instance.Agent.SelectArgs.Clear();
                        MessageSystem<MessageType>.Notify(MessageType.AgentSelectPlayer);
                        MessageSystem<MessageType>.Notify(MessageType.AgentSelectCard);
                        MessageSystem<MessageType>.Notify(MessageType.CloseNewArgsUI);
                    }
                    else
                        //点了别的技能
                        stateMachine.ChangeState<StateSkill>(msg, false, BattleData.Instance.Agent.SelectSkill, paras);
                    break;
                case UIStateMsg.ClickCard:
                case UIStateMsg.ClickPlayer:
                case UIStateMsg.ClickArgs:
                    base.Process(msg, paras);
                    break;
            }
        }
    }
}
