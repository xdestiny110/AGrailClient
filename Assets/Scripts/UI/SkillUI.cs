using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using Framework.Message;

namespace AGrail
{
    public class SkillUI : MonoBehaviour, IMessageListener<MessageType>
    {
        [SerializeField]
        private RawImage skillIcon;
        [SerializeField]
        private Image selectBorder;
        [SerializeField]
        private Text skillName;
        [SerializeField]
        private Button btn;

        private Skill skill;
        public Skill Skill
        {
            set
            {
                skill = value;
                skillName.text = skill.SkillName;
                switch (skill.SkillType)
                {
                    case SkillType.启动:
                        skillIcon.texture = Resources.Load<Texture2D>("Icons/qidong");
                        break;
                    case SkillType.被动:
                        skillIcon.texture = Resources.Load<Texture2D>("Icons/beidong");
                        break;
                    case SkillType.响应:
                        skillIcon.texture = Resources.Load<Texture2D>("Icons/xiangying");
                        break;
                    case SkillType.法术:
                        skillIcon.texture = Resources.Load<Texture2D>("Icons/fashu");
                        break;
                }
                IsEnable = false;                
            }
            get { return skill; }
        }

        public bool IsEnable
        {
            set
            {
                btn.interactable = value;
            }
        }

        void Awake()
        {
            MessageSystem<MessageType>.Regist(MessageType.AgentSelectSkill, this);
        }

        void OnDestroy()
        {
            MessageSystem<MessageType>.UnRegist(MessageType.AgentSelectSkill, this);
        }

        public void OnEventTrigger(MessageType eventType, params object[] parameters)
        {
            switch (eventType)
            {
                case MessageType.AgentSelectCard:
                    if (BattleData.Instance.Agent.SelectSkill == skill.SkillID)
                        selectBorder.enabled = true;
                    else
                        selectBorder.enabled = false;
                    break;
            }
        }

        public void OnBtnClick()
        {
            if (!selectBorder.enabled)            
                BattleData.Instance.Agent.ChangeSelectSkill(skill.SkillID);                            
            else            
                BattleData.Instance.Agent.ChangeSelectSkill(null);                            
        }
    }
}
