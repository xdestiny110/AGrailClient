using System;
using Framework;
using Framework.Message;
using System.Collections.Generic;
using Framework.Network;

namespace AGrail
{
    public class RoleChoose : Singleton<RoleChoose>, IMessageListener<MessageType>
    {
        public network.ROLE_STRATEGY RoleStrategy { get; private set; }
        public network.BP_OPRATION BPopration { get; private set; }
        public uint oprater{ get; private set; }
        public List<uint> RoleIDs { get; private set; }
        public List<int> options { get; private set; }

        public RoleChoose() : base()
        {
            MessageSystem<MessageType>.Regist(MessageType.ROLEREQUEST, this);
        }

        public void Choose(uint roleID)
        {
            bool IsPick = (BPopration == network.BP_OPRATION.BP_BAN) ? false : true ;
            var proto = new network.PickBan() { strategy = (uint)RoleStrategy, is_pick = IsPick };
            proto.role_ids.Add(roleID);
            GameManager.TCPInstance.Send(new Protobuf() { Proto = proto, ProtoID = ProtoNameIds.PICKBAN });
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
                        BPopration = (network.BP_OPRATION)proto.opration;
                        MessageSystem<MessageType>.Notify(MessageType.ChooseRole, RoleStrategy);
                        MessageSystem<MessageType>.Notify(MessageType.PICKBAN);
                    }
                    break;
                default : break;
            }
        }
    }
}


