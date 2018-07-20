using System;
using Framework;
using Framework.Message;
using System.Collections.Generic;
using Framework.Network;

namespace AGrail
{
    public class RoleChoose : Singleton<RoleChoose>, IMessageListener<MessageType>, IMessageListener
    {
        public network.ROLE_STRATEGY RoleStrategy { get; private set; }
        public uint opration { get; private set; }//1234:null,ban,pick,ib
        public uint oprater{ get; private set; }
        public List<uint> RoleIDs { get; private set; }
        public List<int> options { get; private set; }

        public RoleChoose() : base()
        {
            MessageSystem<MessageType>.Regist(MessageType.ROLEREQUEST, this);
        }

        public void Choose(uint roleID)
        {
            bool IsPick;
            if (RoleStrategy == network.ROLE_STRATEGY.ROLE_STRATEGY_BP && RoleStrategy == network.ROLE_STRATEGY.ROLE_STRATEGY_CM)
                IsPick = (opration == 3 ? true : false);
            else IsPick = true;
            var proto = new network.PickBan() { strategy = (uint)RoleStrategy, is_pick = IsPick };
            proto.role_ids.Add(roleID);
            GameManager.TCPInstance.Send(new Protobuf() { Proto = proto, ProtoID = ProtoNameIds.PICKBAN });
        }

        public void OnEventTrigger(string eventType, params object[] parameters)
        {
            if (Array.Exists(Enum.GetNames(typeof(MessageType)), (s) => { return s.Equals(eventType); }))
            {
                OnEventTrigger((MessageType)Enum.Parse(typeof(MessageType), eventType), parameters);
            }
        }

        public void OnEventTrigger(MessageType eventType, params object[] parameters)
        {
            switch(eventType)
            {
                case MessageType.ROLEREQUEST:
                    var proto = parameters[0] as network.RoleRequest;
                    RoleStrategy = proto.strategy;
                    oprater = proto.id;
                    RoleIDs = proto.role_ids;
                    if (RoleStrategy == network.ROLE_STRATEGY.ROLE_STRATEGY_31)
                    {
                        MessageSystem<MessageType>.Notify(MessageType.ChooseRole, RoleStrategy);
                        break;
                    }
                    if(RoleStrategy == network.ROLE_STRATEGY.ROLE_STRATEGY_BP)
                    {
                        options = proto.args;
                        opration = proto.opration;
                        MessageSystem<MessageType>.Notify(MessageType.ChooseRole, RoleStrategy);
                        MessageSystem<MessageType>.Notify(MessageType.PICKBAN);
                        break;
                    }
                    if (RoleStrategy == network.ROLE_STRATEGY.ROLE_STRATEGY_CM)
                    {
                        options = proto.args;
                        switch(proto.opration)
                        {
                            case 1:
                                opration = 1;
                                break;
                            case 2:
                            case 5:
                                opration = 2;
                                break;
                            case 3:
                            case 6:
                                opration = 4;
                                break;
                            case 4:
                            case 7:
                                opration = 3;
                                break;
                        }
                        MessageSystem<MessageType>.Notify(MessageType.ChooseRole, RoleStrategy);
                        MessageSystem<MessageType>.Notify(MessageType.PICKBAN);
                        break;
                    }

                    break;
                default : break;
            }
        }
    }
}


