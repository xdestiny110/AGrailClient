using Framework.FSM;
using Framework.Message;

namespace AGrail
{
    public class StateAttack : UIStateBase
    {
        public override string StateName
        {
            get
            {
                return "Attack";
            }
        }

        public override uint StateNumber
        {
            get
            {
                return 1;
            }
        }

        public StateAttack(StateMachine<StateMsg> machine) : base(machine) { }

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
