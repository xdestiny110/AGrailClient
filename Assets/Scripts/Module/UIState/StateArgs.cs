using Framework.FSM;

namespace AGrail
{
    public class StateArgs : UIStateBase
    {

        public override string StateName
        {
            get
            {
                return "Args";
            }
        }

        public override uint StateNumber
        {
            get
            {
                return 13;
            }
        }

        public StateArgs(StateMachine<StateMsg> machine) : base(machine) { }

        public override void Enter(StateMsg msg, params object[] paras)
        {
            BattleData.Instance.Agent.SelectArgs.Clear();
            base.Enter(msg, paras);
        }

        public override void Process(StateMsg msg, params object[] paras)
        {

        }
    }
}


