using Framework.FSM;
using Framework.Message;
using System;
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
            var canUseHealCount = BattleData.Instance.Agent.Cmd.args[3];
            var harmPoint = BattleData.Instance.Agent.Cmd.args[1];
            var selectList = new List<List<uint>>();
            var explainList = new List<string>();
            //服务器默认玩家有治疗时才会发这个响应
            for (uint i = Math.Min(canUseHealCount, harmPoint); i > 0; i--)
            {
                selectList.Add(new List<uint>() { i });
                explainList.Add(i.ToString() + "个治疗");
            }
            MessageSystem<MessageType>.Notify(MessageType.ShowNewArgsUI, selectList, explainList);
            base.Enter(msg, paras);
        }

        public override void Exit(UIStateMsg msg, params object[] paras)
        {
            MessageSystem<MessageType>.Notify(MessageType.CloseNewArgsUI);
            base.Exit(msg, paras);
        }

        public override void Process(UIStateMsg msg, params object[] paras)
        {
            base.Process(msg, paras);
        }
    }
}


