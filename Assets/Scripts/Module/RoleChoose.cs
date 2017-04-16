using System;
using Framework;
using Framework.Message;
using System.Collections.Generic;

namespace AGrail
{
    public class RoleChoose : Singleton<RoleChoose>, IMessageListener<MessageType>
    {
        public network.ROLE_STRATEGY RoleStrategy { get; private set; }
        public bool CanIChoose { get; private set; }
        public List<uint> RoleIDs { get; private set; }

        public RoleChoose() : base()
        {
            MessageSystem<MessageType>.Regist(MessageType.ROLEREQUEST, this);
        }

        public void Choose(uint roleID)
        {
            var proto = new network.PickBan() { strategy = (uint)RoleStrategy, is_pick = true };
            proto.role_ids.Add(roleID);
            MessageSystem<MessageType>.Notify(MessageType.ChooseRole, true);
        }

        public void OnEventTrigger(MessageType eventType, params object[] parameters)
        {
            switch(eventType)
            {
                case MessageType.ROLEREQUEST:
                    var proto = parameters[0] as network.RoleRequest;
                    RoleStrategy = proto.strategy;
                    CanIChoose = proto.id == BattleData.Instance.PlayerID;
                    RoleIDs = proto.role_ids;
                    MessageSystem<MessageType>.Notify(MessageType.ChooseRole, false);
                    break;
            }
        }
    }
}


