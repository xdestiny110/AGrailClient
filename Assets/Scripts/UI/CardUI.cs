using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using Framework.Message;

namespace AGrail
{
    public class CardUI : MonoBehaviour
    {
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
            }
        }

        public void OnCardClick()
        {
            selectBorder.enabled = !selectBorder.enabled;
            MessageSystem<MessageType>.Notify(MessageType.AgentSelectCard, card.ID);            
        }

    }
}


