using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using Framework.Message;
using System;
using System.Collections.Generic;

namespace AGrail
{
    public class PlayerAgentUI : MonoBehaviour, IMessageListener<MessageType>
    {
        [SerializeField]
        private Transform handArea;
        [SerializeField]
        private Transform skillArea;
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
        [SerializeField]
        private GameObject skillPrefab;

        private Dictionary<int, PlayerStatusQT> players;
        private List<SkillUI> skillUIs = new List<SkillUI>();

        void Awake()
        {
            players = GetComponent<BattleUIQT>().PlayersStatus;

            MessageSystem<MessageType>.Regist(MessageType.AgentUpdate, this);
            MessageSystem<MessageType>.Regist(MessageType.AgentSetOKCallback, this);
            MessageSystem<MessageType>.Regist(MessageType.AgentSetCancelCallback, this);
            MessageSystem<MessageType>.Regist(MessageType.AgentHandChange, this);
            MessageSystem<MessageType>.Regist(MessageType.AgentStateChange, this);
            MessageSystem<MessageType>.Regist(MessageType.AgentSelectSkill, this);            

            //先将确认键初始化为准备按钮
            if (BattleData.Instance.PlayerID != 9)
            {
                btnOK.onClick.RemoveAllListeners();
                btnOK.onClick.AddListener(() => 
                {
                    BattleData.Instance.Ready(BattleData.Instance.MainPlayer.ready ? false : true);                    
                });
                btnOK.gameObject.SetActive(true);
                btnCancel.gameObject.SetActive(true);               
                btnCancel.interactable = false;
            }
            else
            {
                btnOK.interactable = false;
                btnCancel.interactable = false;
            }
        }

        void OnDestroy()
        {
            MessageSystem<MessageType>.UnRegist(MessageType.AgentUpdate, this);
            MessageSystem<MessageType>.UnRegist(MessageType.AgentSetOKCallback, this);
            MessageSystem<MessageType>.UnRegist(MessageType.AgentSetCancelCallback, this);
            MessageSystem<MessageType>.UnRegist(MessageType.AgentHandChange, this);
            MessageSystem<MessageType>.UnRegist(MessageType.AgentStateChange, this);
            MessageSystem<MessageType>.UnRegist(MessageType.AgentSelectSkill, this);
        }

        public void OnEventTrigger(MessageType eventType, params object[] parameters)
        {
            switch (eventType)
            {
                case MessageType.AgentUpdate:
                    btnOK.interactable = false;
                    btnOK.onClick.RemoveAllListeners();
                    //初始化技能键
                    foreach (var v in BattleData.Instance.Agent.PlayerRole.Skills.Values)
                    {
                        var go = Instantiate(skillPrefab);
                        go.transform.SetParent(skillArea);
                        go.transform.localPosition = Vector3.zero;
                        go.transform.localRotation = Quaternion.identity;
                        go.transform.localScale = Vector3.one;
                        skillUIs.Add(go.GetComponent<SkillUI>());
                        skillUIs[skillUIs.Count - 1].Skill = v;
                    }
                    break;
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
                case MessageType.AgentSetOKCallback:
                    btnOK.onClick.RemoveAllListeners();
                    if ((bool)parameters[0])
                    {
                        btnOK.interactable = true;
                        btnOK.onClick.AddListener((UnityEngine.Events.UnityAction)parameters[1]);
                    }
                    else
                        btnOK.interactable = false;
                    break;
                case MessageType.AgentSetCancelCallback:
                    btnCancel.onClick.RemoveAllListeners();
                    if ((bool)parameters[0])
                    {
                        btnCancel.interactable = true;
                        btnCancel.onClick.AddListener((UnityEngine.Events.UnityAction)parameters[1]);
                    }
                    else
                        btnCancel.interactable = false;
                    break;
                case MessageType.AgentStateChange:
                    //卡牌、人物是否能够选中
                    //确认、取消、特殊行动按钮能否显示
                    //技能能否选中
                    //未来可以改成利用rolebase具体确定哪些能够选中
                    if(BattleData.Instance.Agent.AgentState.Check(PlayerAgentState.Idle))
                    {
                        for(int i = 0; i < handArea.childCount; i++)                        
                            handArea.GetChild(i).GetComponent<CardUI>().IsEnable = false;
                        foreach(var v in players.Values)                        
                            v.IsEnable = false;
                        foreach (var v in skillUIs)
                            v.IsEnable = false;
                        btnOK.interactable = false;
                        btnCancel.interactable = false;
                        btnBuy.interactable = false;
                        btnExtract.interactable = false;
                        btnSynthetize.interactable = false;
                    }                    
                    else
                    {
                        BattleData.Instance.Agent.PlayerRole.Check(BattleData.Instance.Agent.AgentState);
                        for (int i = 0; i < handArea.childCount; i++)
                            handArea.GetChild(i).GetComponent<CardUI>().IsEnable = true;
                        foreach (var v in players.Values)
                            v.IsEnable = true;
                        foreach (var v in skillUIs)
                            v.IsEnable = true;
                        if (BattleData.Instance.Agent.AgentState.Check(PlayerAgentState.CanSpecial) &&
                            BattleData.Instance.MainPlayer.max_hand - BattleData.Instance.MainPlayer.hand_count >= 3)
                            btnBuy.interactable = true;
                        if (BattleData.Instance.Agent.AgentState.Check(PlayerAgentState.CanSpecial) &&
                            BattleData.Instance.MainPlayer.max_hand - BattleData.Instance.MainPlayer.hand_count >= 3 &&
                            BattleData.Instance.Gem[(int)BattleData.Instance.MainPlayer.team] + BattleData.Instance.Crystal[(int)BattleData.Instance.MainPlayer.team] >= 3)                        
                            btnSynthetize.interactable = true;
                        if (BattleData.Instance.Agent.AgentState.Check(PlayerAgentState.CanSpecial) &&
                            BattleData.Instance.MainPlayer.crystal + BattleData.Instance.MainPlayer.gem < BattleData.Instance.Agent.PlayerRole.MaxEnergyCount &&
                            BattleData.Instance.Gem[(int)BattleData.Instance.MainPlayer.team] + BattleData.Instance.Crystal[(int)BattleData.Instance.MainPlayer.team] > 0)
                            btnExtract.interactable = true;                        
                    }
                    break;
                case MessageType.AgentSelectSkill:
                    //一次只能选中一个技能
                    var skillID = (uint)parameters[0];
                    foreach(var v in skillUIs)
                    {
                        if (v.Skill.SkillID != skillID)
                        {
                            v.IsEnable = false;
                            v.IsEnable = true;
                        }
                    }
                    break;
            }
        }

        public void OnBtnBuyClick()
        {
            GameManager.UIInstance.PushWindow(Framework.UI.WindowType.ChooseEnergy, Framework.UI.WinMsg.Pause, Vector3.zero,
                new Action<uint, uint>((gem, crystal) => { BattleData.Instance.Agent.PlayerRole.Buy(gem, crystal); }),
                new Func<uint, uint, bool>((gem, crystal) => { return BattleData.Instance.Agent.PlayerRole.CheckBuy(gem, crystal); }),
                "购买");
        }

        public void OnBtnExtractClick()
        {
            GameManager.UIInstance.PushWindow(Framework.UI.WindowType.ChooseEnergy, Framework.UI.WinMsg.Pause, Vector3.zero,
                new Action<uint, uint>((gem, crystal) => { BattleData.Instance.Agent.PlayerRole.Extract(gem, crystal); }),
                new Func<uint, uint, bool>((gem, crystal) => { return BattleData.Instance.Agent.PlayerRole.CheckExtract(gem, crystal, BattleData.Instance.MainPlayer.crystal + BattleData.Instance.MainPlayer.gem); }),
                "提炼");
        }

        public void OnBtnSynthetizeClick()
        {
            GameManager.UIInstance.PushWindow(Framework.UI.WindowType.ChooseEnergy, Framework.UI.WinMsg.Pause, Vector3.zero,
                new Action<uint, uint>((gem, crystal) => { BattleData.Instance.Agent.PlayerRole.Synthetize(gem, crystal); }),
                new Func<uint, uint, bool>((gem, crystal) => { return BattleData.Instance.Agent.PlayerRole.CheckSynthetize(gem, crystal); }),
                "合成");
        }
    }
}


