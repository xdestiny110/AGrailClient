﻿using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using Framework.Message;
using System;

namespace AGrail
{
    public class CardUI : MonoBehaviour,  IMessageListener<MessageType>
    {
        [SerializeField]
        private Canvas canvas;
        [SerializeField]
        private Button btn;
        [SerializeField]
        private Text txtSkill1;
        [SerializeField]
        private Text txtSkill2;
        [SerializeField]
        private RawImage image;
        [SerializeField]
        private Image selectBorder;

        public bool IsEnable
        {
            set; get;
        }

        private Card card;
        public Card Card
        {
            set
            {
                card = value;
                image.texture = Resources.Load<Texture2D>(card.AssetPath);
                if (card.SkillNum >= 1)
                {
                    txtSkill1.text = card.SkillNames[0];
                    if (card.SkillNum >= 2)
                        txtSkill2.text = card.SkillNames[1];
                }
                IsEnable = false;
            }
            get { return card; }
        }

        void Awake()
        {
            MessageSystem<MessageType>.Regist(MessageType.AgentSelectCard, this);
        }

        void Destroy()
        {
            MessageSystem<MessageType>.UnRegist(MessageType.AgentSelectCard, this);
        }

        public void OnEventTrigger(MessageType eventType, params object[] parameters)
        {
            switch (eventType)
            {
                case MessageType.AgentSelectCard:
                    if (BattleData.Instance.Agent.SelectCards.Contains(card.ID))
                        selectBorder.enabled = true;
                    else
                        selectBorder.enabled = false;
                    break;
            }
        }

        public void OnCardClick()
        {
            if (IsEnable)
            {
                if (!selectBorder.enabled)
                    BattleData.Instance.Agent.AddSelectCard(card.ID);
                else
                    BattleData.Instance.Agent.RemoveSelectCard(card.ID);
            }    
        }

        public void OnPointerEnter(BaseEventData eventData)
        {
            var pos = canvas.transform.localPosition;
            pos.y += 10;
            canvas.transform.localPosition = pos;
            canvas.overrideSorting = true;
            canvas.sortingOrder = 10; 
        }

        public void OnPointerExit(BaseEventData eventData)
        {
            var pos = canvas.transform.localPosition;
            pos.y = 0;
            canvas.transform.localPosition = pos;
            canvas.overrideSorting = false;
            canvas.sortingOrder = 0;
        }
    }
}


