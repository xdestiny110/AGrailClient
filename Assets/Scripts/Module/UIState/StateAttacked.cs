using UnityEngine;
using System.Collections;
using Framework.FSM;
using System;
using Framework.Message;

namespace AGrail
{
    public class StateAttacked : UIStateBase
    {
        public override string StateName
        {
            get
            {
                return "Attacked";
            }
        }

        public override uint StateNumber
        {
            get
            {
                return 3;
            }
        }

        public StateAttacked(StateMachine<UIStateMsg> machine) : base(machine) { }

        public override void Process(UIStateMsg msg, params object[] paras)
        {
            switch (msg)
            {
                case UIStateMsg.ClickCard:
                case UIStateMsg.ClickPlayer:
                    if(BattleData.Instance.Agent.SelectCards.Count == 1 &&
                        Card.GetCard(BattleData.Instance.Agent.SelectCards[0]).Element == Card.CardElement.light)
                    {
                        BattleData.Instance.Agent.SelectPlayers.Clear();
                        MessageSystem<MessageType>.Notify(MessageType.AgentSelectPlayer);
                    }
                    base.Process(msg, paras);
                    break;
                default:
                    base.Process(msg, paras);
                    break;
            }
        }
    }
}


