using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using Framework.Message;
using System;
using DG.Tweening;
using Framework.AssetBundle;

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
        private Image image;
        [SerializeField]
        private Transform propertyRoot;
        [SerializeField]
        private Transform elementRoot;
        [SerializeField]
        private Image selectBorder;
        [SerializeField]
        private Image canSelectBorder;

        private bool isEnable;
        public bool IsEnable
        {
            set
            {
                isEnable = value;
                if (!isEnable)
                {
                    image.color = new Color(0.5f, 0.5f, 0.5f, 1);
                    for (int i = 0; i < propertyRoot.childCount; i++)
                        propertyRoot.GetChild(i).GetComponent<Image>().color = new Color(0.5f, 0.5f, 0.5f, 1);
                    for (int i = 0; i < elementRoot.childCount; i++)
                        elementRoot.GetChild(i).GetComponent<Image>().color = new Color(0.5f, 0.5f, 0.5f, 1);
                    canSelectBorder.enabled = false;
                }
                else
                {
                    image.color = Color.white;
                    for (int i = 0; i < propertyRoot.childCount; i++)
                        propertyRoot.GetChild(i).GetComponent<Image>().color = Color.white;
                    for (int i = 0; i < elementRoot.childCount; i++)
                        elementRoot.GetChild(i).GetComponent<Image>().color = Color.white;
                    canSelectBorder.enabled = true;
                }
            }
            get
            {
                return isEnable;
            }
        }

        private Card card;
        public Card Card
        {
            set
            {
                card = value;
                image.sprite = AssetBundleManager.Instance.LoadAsset<Sprite>("card", card.AssetPath);
                for (int i = 0; i < propertyRoot.childCount; i++)
                    propertyRoot.GetChild(i).gameObject.SetActive(false);
                if (card.Property != Card.CardProperty.无)
                    propertyRoot.Find(card.Property.ToString()).gameObject.SetActive(true);
                for (int i = 0; i < elementRoot.childCount; i++)
                    elementRoot.GetChild(i).gameObject.SetActive(false);
                if (card.Element != Card.CardElement.darkness && card.Element != Card.CardElement.light && card.Element != Card.CardElement.none)
                    elementRoot.Find(card.Element.ToString()).gameObject.SetActive(true);
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

        void OnDestroy()
        {
            MessageSystem<MessageType>.UnRegist(MessageType.AgentSelectCard, this);
        }

        public void OnEventTrigger(MessageType eventType, params object[] parameters)
        {
            switch (eventType)
            {
                case MessageType.AgentSelectCard:
                    if (BattleData.Instance.Agent.SelectCards.Contains(card.ID))
                    {
                        selectBorder.enabled = true;
                        canSelectBorder.enabled = false;
                    }
                    else
                    {
                        selectBorder.enabled = false;
                        canSelectBorder.enabled = IsEnable;
                    }
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

        private bool isDisappear = false;
        public void Disappear()
        {
            isDisappear = true;
            Invoke("disappear", 3);
        }

        public void OnPointerEnter(BaseEventData eventData)
        {
            if (isDisappear) return;
            var pos = canvas.transform.localPosition;
            pos.y += 10;
            canvas.transform.localPosition = pos;
            canvas.overrideSorting = true;
            canvas.sortingOrder = 10;
        }

        public void OnPointerExit(BaseEventData eventData)
        {
            if (isDisappear) return;
            var pos = canvas.transform.localPosition;
            pos.y = 0;
            canvas.transform.localPosition = pos;
            canvas.overrideSorting = false;
            canvas.sortingOrder = 0;
        }

        private void disappear()
        {
            DOTween.To(() => image.color, x => image.color = x, new Color(1, 1, 1, 0), 0.6f).SetOptions(true)
                .OnComplete(()=> { Destroy(this.gameObject); });
            DOTween.To(() => txtSkill1.color, x => txtSkill1.color = x, new Color(1, 1, 1, 0), 0.5f).SetOptions(true);
            DOTween.To(() => txtSkill2.color, x => txtSkill2.color = x, new Color(1, 1, 1, 0), 0.5f).SetOptions(true);
            var images = gameObject.GetComponentsInChildren<Image>();
            foreach (var v in images)
                DOTween.To(() => v.color, x => v.color = x, new Color(1, 1, 1, 0), 0.5f).SetOptions(true);
        }
    }
}


