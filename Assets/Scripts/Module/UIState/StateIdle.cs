using UnityEngine;
using System.Collections;
using Framework.FSM;
using System;
using Framework.Message;

namespace AGrail
{
    public class StateIdle : UIStateBase
    {
        public override string StateName
        {
            get
            {
                return "Idle";
            }
        }

        public override uint StateNumber
        {
            get
            {
                return 0;
            }
        }

        public StateIdle(StateMachine<StateMsg> machine) : base(machine) { }

        public override void Enter(StateMsg msg, params object[] paras)
        {            
            BattleData.Instance.Agent.SelectCards.Clear();
            BattleData.Instance.Agent.SelectPlayers.Clear();
            BattleData.Instance.Agent.SelectSkill = null;
            MessageSystem<MessageType>.Notify(MessageType.AgentSelectPlayer);
            MessageSystem<MessageType>.Notify(MessageType.AgentSelectCard);
            MessageSystem<MessageType>.Notify(MessageType.AgentSelectSkill);
            base.Enter(msg, paras);
        }

        public override void Process(StateMsg msg, params object[] paras)
        {
            if(msg == StateMsg.Init)
            {
                stateMachine.ChangeState((Type)paras[0], true, msg, paras);
            }
        }
    }
}