using System;
using Framework;
using Framework.UI;
using Framework.Message;

namespace AGrail
{
    public class BattleUI : WindowsBase
    {
        public override WindowType Type
        {
            get
            {
                return WindowType.Battle;
            }
        }

        public override void Awake()
        {
            MessageSystem<MessageType>.Regist(MessageType.MoraleChange, this);
            MessageSystem<MessageType>.Regist(MessageType.GemChange, this);
            MessageSystem<MessageType>.Regist(MessageType.CrystalChange, this);
            MessageSystem<MessageType>.Regist(MessageType.GrailChange, this);
            MessageSystem<MessageType>.Regist(MessageType.PlayerIsReady, this);
            MessageSystem<MessageType>.Regist(MessageType.PlayerNickName, this);
            base.Awake();
        }

        public override void OnDestroy()
        {

            base.OnDestroy();
        }

        public override void OnEventTrigger(MessageType eventType, params object[] parameters)
        {
            switch (eventType)
            {

            }
        }

    }
}


