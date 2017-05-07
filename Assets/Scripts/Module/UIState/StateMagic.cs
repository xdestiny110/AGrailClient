using Framework.FSM;

namespace AGrail
{
    public class StateMagic : UIStateBase
    {
        public override string StateName
        {
            get
            {
                return "Magic";
            }
        }

        public override uint StateNumber
        {
            get
            {
                return 2;
            }
        }

        public StateMagic(StateMachine<StateMsg> machine) : base(machine) { }

        public override void Process(StateMsg msg, params object[] paras)
        {
            switch (msg)
            {
                case StateMsg.ClickCard:
                    if (BattleData.Instance.Agent.SelectCards.Count == 0)
                        stateMachine.ChangeState(stateMachine.Parent.GetType(), true, StateMsg.Init, paras);
                    break;
            }
        }
    }
}
