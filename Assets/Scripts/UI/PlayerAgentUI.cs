using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using Framework.Message;
using System;

namespace AGrail
{
    public class PlayerAgentUI : MonoBehaviour, IMessageListener<MessageType>
    {
        [SerializeField]
        private Transform handArea;
        [SerializeField]
        private Button btnOK;
        [SerializeField]
        private Button btnCancel;
        [SerializeField]
        private Button btnBuy;
        [SerializeField]
        private Button btnExtract;
        [SerializeField]
        private Button btnSynthetize;

        void Awake()
        {
            MessageSystem<MessageType>.Regist(MessageType.AgentSetOKCallback, this);
            MessageSystem<MessageType>.Regist(MessageType.AgentSetCancelCallback, this);
            //先将确认键初始化为准备按钮
            if(BattleData.Instance.PlayerID != 9)
            {
                btnOK.onClick.RemoveAllListeners();
                btnOK.onClick.AddListener(() => 
                {
                    BattleData.Instance.Ready(BattleData.Instance.MainPlayer.ready ? false : true);                    
                });
                btnOK.gameObject.SetActive(true);
                btnCancel.gameObject.SetActive(false);
            }
            else
            {
                btnOK.gameObject.SetActive(false);
                btnCancel.gameObject.SetActive(false);
            }
        }

        void OnDestroy()
        {
            MessageSystem<MessageType>.UnRegist(MessageType.AgentSetOKCallback, this);
            MessageSystem<MessageType>.UnRegist(MessageType.AgentSetCancelCallback, this);
        }

        public void OnEventTrigger(MessageType eventType, params object[] parameters)
        {
            switch (eventType)
            {

            }
        }
    }
}


