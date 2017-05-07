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
                    skillID = (uint)paras[0];
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
            }                
            base.Enter(msg, paras);
        }

        public override void Process(UIStateMsg msg, params object[] paras)
        {
            switch(msg)
            {
                case UIStateMsg.ClickSkill:
                    if (BattleData.Instance.Agent.SelectSkill == null)
                        stateMachine.BackState(msg, paras);                        
                    else
                        //点了别的技能
                        stateMachine.ChangeState<StateSkill>(msg, false, BattleData.Instance.Agent.SelectSkill, paras);
                    break;
                case UIStateMsg.ClickCard:
                    clickCard();
                    break;
                case UIStateMsg.ClickPlayer:
                    clickPlayer();
                    break;
            }
        }

        private void clickCard()
        {
            //所有人物技能的特定动作(主要就是那些需要额外args的)
            //如果不用切换状态，需要进行base.Process(msg, paras);
            switch (BattleData.Instance.MainPlayer.role_id)
            {

            }
        }

        private void clickPlayer()
        {
            //所有人物技能的特定动作(主要就是那些需要额外args的)
            //如果不用切换状态，需要进行base.Process(msg, paras);
            switch (BattleData.Instance.MainPlayer.role_id)
            {

            }
        }

    }
}
