using Framework.FSM;
using Framework.Message;
using System.Collections.Generic;

namespace AGrail
{
    public class StateHealCost : UIStateBase
    {

        public override string StateName
        {
            get
            {
                return "HealCost";
            }
        }

        public override uint StateNumber
        {
            get
            {
                return 7;
            }
        }

        public StateHealCost(StateMachine<UIStateMsg> machine) : base(machine) { }

        public override void Enter(UIStateMsg msg, params object[] paras)
        {
            var selectList = new List<List<uint>>();
            //服务器默认玩家有治疗时才会发这个响应
            for (uint i = 1; i <= BattleData.Instance.MainPlayer.heal_count; i++)
                selectList.Add(new List<uint>() { i });
            MessageSystem<MessageType>.Notify(MessageType.ShowArgsUI, "Heal", selectList);
            base.Enter(msg, paras);
        }

        public override void Exit(UIStateMsg msg, params object[] paras)
        {
            MessageSystem<MessageType>.Notify(MessageType.CloseArgsUI);
            base.Exit(msg, paras);
        }

        public override void Process(UIStateMsg msg, params object[] paras)
        {

        }
    }
}


