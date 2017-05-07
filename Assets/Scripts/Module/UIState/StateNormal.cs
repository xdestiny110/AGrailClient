using Framework.FSM;
using Framework.Message;

namespace AGrail
{
    public class StateNormal : UIStateBase
    {
        public override string StateName
        {
            get
            {
                return "Normal";
            }
        }

        public override uint StateNumber
        {
            get
            {
                return 10;
            }
        }

        public StateNormal(StateMachine<StateMsg> machine) : base(machine) { }

        public override void Enter(StateMsg msg, params object[] paras)
        {
            if(msg == StateMsg.Init)
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

        public override void Process(StateMsg msg, params object[] paras)
        {
            switch (msg)
            {
                case StateMsg.ClickCard:
                    if (Card.GetCard(BattleData.Instance.Agent.SelectCards[0]).Type == Card.CardType.attack)
                        stateMachine.ChangeState<StateAttack>(msg, true, paras);
                    else
                        stateMachine.ChangeState<StateMagic>(msg, true, paras);
                    break;
                case StateMsg.ClickSkill:
                    stateMachine.ChangeState<StateSkill>(msg, true, BattleData.Instance.Agent.SelectSkill.Value, paras);
                    break;
                case StateMsg.ClickBtn:
                    var cmd = paras[0].ToString();
                    if (cmd == "Buy")
                    {
                        stateMachine.ChangeState<StateArgs>(msg, true);
                    }
                    if(cmd == "Extract")
                    {

                    }
                    if(cmd == "Syntheis")
                    {

                    }
                    break;
            }
        }
    }
}
