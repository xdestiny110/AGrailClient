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
        [SerializeField]
        private GameObject cardPrefab;

        void Awake()
        {
            MessageSystem<MessageType>.Regist(MessageType.AgentSetOKCallback, this);
            MessageSystem<MessageType>.Regist(MessageType.AgentSetCancelCallback, this);
            MessageSystem<MessageType>.Regist(MessageType.AgentHandChange, this);
            
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
            MessageSystem<MessageType>.UnRegist(MessageType.AgentHandChange, this);
        }

        public void OnEventTrigger(MessageType eventType, params object[] parameters)
        {
            switch (eventType)
            {
                case MessageType.AgentHandChange:
                    for(int i = 0; i < handArea.childCount; i++)                    
                        Destroy(handArea.GetChild(i).gameObject);                    
                    foreach(var v in BattleData.Instance.MainPlayer.hands)
                    {
                        var go = Instantiate(cardPrefab);
                        go.transform.SetParent(handArea);
                        go.transform.localPosition = Vector3.zero;
                        go.transform.localRotation = Quaternion.identity;
                        go.transform.localScale = Vector3.one;
                        var cardUI = go.GetComponent<CardUI>();
                        cardUI.Card = new Card(v);
                    }                    
                    break;
            }
        }
    }
}


