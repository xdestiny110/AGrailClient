using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using Framework.Message;
using Framework.AssetBundle;
using System;

namespace AGrail
{
    public class SkillUI : MonoBehaviour, IMessageListener<MessageType>, IMessageListener
    {
        [SerializeField]
        private Image skillIcon;
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
                var sprite = AssetBundleManager.Instance.LoadAsset<Sprite>("skill_texture", skill.SkillName);
                if (sprite != null)
                    skillIcon.sprite = sprite;
                IsEnable = false;
            }
            get { return skill; }
        }

        public bool IsEnable
        {
            set
            {
                gameObject.SetActive(value);
            }
        }

        void Awake()
        {
            btn.onClick.AddListener(onBtnClick);

            MessageSystem<MessageType>.Regist(MessageType.AgentSelectSkill, this);
        }

        void OnDestroy()
        {
            MessageSystem<MessageType>.UnRegist(MessageType.AgentSelectSkill, this);
        }

        public void OnEventTrigger(string eventType, params object[] parameters)
        {
            if (Array.Exists(Enum.GetNames(typeof(MessageType)), (s) => { return s.Equals(eventType); }))
            {
                OnEventTrigger((MessageType)Enum.Parse(typeof(MessageType), eventType));
            }
        }

        public void OnEventTrigger(MessageType eventType, params object[] parameters)
        {
            switch (eventType)
            {
                case MessageType.AgentSelectSkill:
                    if (BattleData.Instance.Agent.SelectSkill == skill.SkillID)
                        selectBorder.enabled = true;
                    else
                        selectBorder.enabled = false;
                    break;
            }
        }

        private void onBtnClick()
        {
            if (!selectBorder.enabled)
                BattleData.Instance.Agent.ChangeSelectSkill(skill.SkillID);
            else
                BattleData.Instance.Agent.ChangeSelectSkill(null);
        }
    }
}
