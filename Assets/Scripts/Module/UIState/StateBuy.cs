using Framework.FSM;

namespace AGrail
{
    public class StateBuy : UIStateBase
    {
        public override string StateName
        {
            get
            {
                return "Buy";
            }
        }

        public override uint StateNumber
        {
            get
            {
                return 12;
            }
        }

        public StateBuy(StateMachine<UIStateMsg> machine) : base(machine) { }

    }
}
