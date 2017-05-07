using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using Framework.Message;
using System;

namespace AGrail
{
    public class CardUI : MonoBehaviour
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

        private bool isEnable = false;
        public bool IsEnable
        {
            set
            {
                isEnable = value;
                if (!isEnable)
                {
                    selectBorder.enabled = false;
                    btn.interactable = false;
                    BattleData.Instance.Agent.SelectCards.Remove(card.ID);
                }
                else
                    btn.interactable = true;
            }
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
                IsEnable = true;
            }
            get { return card; }
        }

        public void OnCardClick()
        {
            if (isEnable)
            {
                selectBorder.enabled = !selectBorder.enabled;
                if (selectBorder.enabled)
                {
                    BattleData.Instance.Agent.SelectCards.Add(card.ID);
                    if (BattleData.Instance.Agent.AgentUIState == 10 || BattleData.Instance.Agent.AgentUIState == 11)
                        BattleData.Instance.Agent.AgentUIState = (card.Type == Card.CardType.attack) ? (uint)1 : 2;
                }
                else
                {
                    BattleData.Instance.Agent.SelectCards.Remove(card.ID);
                    BattleData.Instance.Agent.AgentState = BattleData.Instance.Agent.AgentState;
                }               
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


