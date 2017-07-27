using Framework.FSM;
using Framework.Message;

namespace AGrail
{
    public class StateAttackAndMagic : UIStateBase
    {
        public override string StateName
        {
            get
            {
                return "AttackAndMagic";
            }
        }

        public override uint StateNumber
        {
            get
            {
                return 11;
            }
        }

        public StateAttackAndMagic(StateMachine<UIStateMsg> machine) : base(machine) { }

        public override void Enter(UIStateMsg msg, params object[] paras)
        {
            if (msg == UIStateMsg.Init)
            {
                BattleData.Instance.Agent.SelectCards.Clear();
                BattleData.Instance.Agent.SelectPlayers.Clear();
                BattleData.Instance.Agent.SelectSkill = null;
                BattleData.Instance.Agent.SelectArgs.Clear();
                MessageSystem<MessageType>.Notify(MessageType.AgentSelectPlayer);
                MessageSystem<MessageType>.Notify(MessageType.AgentSelectCard);
                MessageSystem<MessageType>.Notify(MessageType.AgentSelectSkill);
            }
            base.Enter(msg, paras);
        }

        public override void Process(UIStateMsg msg, params object[] paras)
        {
            switch (msg)
            {
                case UIStateMsg.ClickCard:
                    if (Card.GetCard(BattleData.Instance.Agent.SelectCards[0]).Type == Card.CardType.attack)
                        stateMachine.ChangeState<StateAttack>(msg, true, paras);
                    else
                        stateMachine.ChangeState<StateMagic>(msg, true, paras);
                    break;
                case UIStateMsg.ClickSkill:
                    stateMachine.ChangeState<StateSkill>(msg, true, BattleData.Instance.Agent.SelectSkill.Value, paras);
                    break;
                default:
                    base.Process(msg, paras);
                    break;
            }
        }
    }
}
