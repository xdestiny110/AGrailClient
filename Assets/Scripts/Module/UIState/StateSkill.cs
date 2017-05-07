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

        public StateSkill(StateMachine<StateMsg> machine) : base(machine) { }

        public override void Enter(StateMsg msg, params object[] paras)
        {
            skillID = (uint)paras[0];
            BattleData.Instance.Agent.SelectCards.Clear();
            BattleData.Instance.Agent.SelectPlayers.Clear();
            BattleData.Instance.Agent.SelectArgs.Clear();
            MessageSystem<MessageType>.Notify(MessageType.AgentSelectPlayer);
            MessageSystem<MessageType>.Notify(MessageType.AgentSelectCard);
            //响应技能
            if(msg == StateMsg.Init)
                BattleData.Instance.Agent.SelectSkill = BattleData.Instance.Agent.Cmd.respond_id;            
            base.Enter(msg, paras);
        }

        public override void Process(StateMsg msg, params object[] paras)
        {
            switch(msg)
            {
                case StateMsg.ClickSkill:
                    if (BattleData.Instance.Agent.SelectSkill == null)
                        stateMachine.ChangeState(stateMachine.Parent.GetType(), true, msg, paras);
                    else
                        //点了别的技能
                        stateMachine.ChangeState<StateSkill>(msg, false, BattleData.Instance.Agent.SelectSkill, paras);
                    break;
                case StateMsg.ClickCard:
                    clickCard();
                    break;
                case StateMsg.ClickPlayer:
                    clickPlayer();
                    break;                    
            }
        }

        private void clickCard()
        {
            //所有人物技能的特定动作(主要就是那些需要额外args的)

        }

        private void clickPlayer()
        {
            //所有人物技能的特定动作(主要就是那些需要额外args的)

        }

    }
}
