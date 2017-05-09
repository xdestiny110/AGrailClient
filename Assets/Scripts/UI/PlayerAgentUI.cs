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
        private List<CardUI> cardUIs = new List<CardUI>();

        void Awake()
        {
            players = GetComponent<BattleUIQT>().PlayersStatus;

            MessageSystem<MessageType>.Regist(MessageType.AgentUpdate, this);
            MessageSystem<MessageType>.Regist(MessageType.AgentHandChange, this);
            MessageSystem<MessageType>.Regist(MessageType.AgentStateChange, this);
            MessageSystem<MessageType>.Regist(MessageType.AgentSelectSkill, this);
            MessageSystem<MessageType>.Regist(MessageType.AgentUIStateChange, this);
            MessageSystem<MessageType>.Regist(MessageType.ShowArgsUI, this);
            MessageSystem<MessageType>.Regist(MessageType.CloseArgsUI, this);

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
            MessageSystem<MessageType>.UnRegist(MessageType.AgentHandChange, this);
            MessageSystem<MessageType>.UnRegist(MessageType.AgentStateChange, this);
            MessageSystem<MessageType>.UnRegist(MessageType.AgentSelectSkill, this);
            MessageSystem<MessageType>.UnRegist(MessageType.AgentUIStateChange, this);
            MessageSystem<MessageType>.UnRegist(MessageType.ShowArgsUI, this);
        }

        public void OnEventTrigger(MessageType eventType, params object[] parameters)
        {
            switch (eventType)
            {
                case MessageType.AgentUIStateChange:
                    //UI状态变化，确认哪些能够选择
                    foreach (var v in cardUIs)
                    {
                        if (BattleData.Instance.Agent.PlayerRole.CanSelect(BattleData.Instance.Agent.FSM.Current.StateNumber, v.Card))
                            v.IsEnable = true;
                        else
                            v.IsEnable = false;
                    }
                    foreach (var v in skillUIs)
                    {
                        if (BattleData.Instance.Agent.PlayerRole.CanSelect(BattleData.Instance.Agent.FSM.Current.StateNumber, v.Skill))
                            v.IsEnable = true;
                        else
                            v.IsEnable = false;
                    }
                    foreach (var v in players.Keys)
                    {
                        if (BattleData.Instance.Agent.PlayerRole.CanSelect(BattleData.Instance.Agent.FSM.Current.StateNumber, BattleData.Instance.PlayerInfos[v]))
                            players[v].IsEnable = true;
                        else
                            players[v].IsEnable = false;
                    }
                    btnOK.interactable = BattleData.Instance.Agent.PlayerRole.CheckOK(BattleData.Instance.Agent.FSM.Current.StateNumber,
                        BattleData.Instance.Agent.SelectCards, BattleData.Instance.Agent.SelectPlayers, BattleData.Instance.Agent.SelectSkill);
                    btnCancel.interactable = BattleData.Instance.Agent.PlayerRole.CheckCancel(BattleData.Instance.Agent.FSM.Current.StateNumber,
                        BattleData.Instance.Agent.SelectCards, BattleData.Instance.Agent.SelectPlayers, BattleData.Instance.Agent.SelectSkill);
                    btnBuy.interactable = BattleData.Instance.Agent.PlayerRole.CheckBuy(BattleData.Instance.Agent.FSM.Current.StateNumber);
                    btnExtract.interactable = BattleData.Instance.Agent.PlayerRole.CheckExtract(BattleData.Instance.Agent.FSM.Current.StateNumber);
                    btnSynthetize.interactable = BattleData.Instance.Agent.PlayerRole.CheckSynthetize(BattleData.Instance.Agent.FSM.Current.StateNumber);
                    break;
                case MessageType.AgentUpdate:
                    btnOK.interactable = false;
                    btnOK.onClick.RemoveAllListeners();
                    btnOK.onClick.AddListener(onBtnOKClick);
                    btnCancel.onClick.RemoveAllListeners();
                    btnCancel.onClick.AddListener(onBtnCancelClick);
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
                    btnBuy.interactable = BattleData.Instance.Agent.PlayerRole.CheckBuy(BattleData.Instance.Agent.FSM.Current.StateNumber);
                    btnExtract.interactable = BattleData.Instance.Agent.PlayerRole.CheckExtract(BattleData.Instance.Agent.FSM.Current.StateNumber);                    
                    btnSynthetize.interactable = BattleData.Instance.Agent.PlayerRole.CheckSynthetize(BattleData.Instance.Agent.FSM.Current.StateNumber);
                    break;
                case MessageType.AgentHandChange:
                    for(int i = 0; i < handArea.childCount; i++)                    
                        Destroy(handArea.GetChild(i).gameObject);
                    cardUIs.Clear();
                    foreach(var v in BattleData.Instance.MainPlayer.hands)
                    {
                        var go = Instantiate(cardPrefab);
                        go.transform.SetParent(handArea);
                        go.transform.localPosition = Vector3.zero;
                        go.transform.localRotation = Quaternion.identity;
                        go.transform.localScale = Vector3.one;
                        var cardUI = go.GetComponent<CardUI>();
                        cardUI.Card = Card.GetCard(v);
                        cardUIs.Add(cardUI);
                    }
                    break;
                case MessageType.AgentStateChange:
                    //保证在初始状态 
                    foreach (var v in cardUIs)
                            v.IsEnable = false;
                    foreach (var v in skillUIs)
                            v.IsEnable = false;
                    foreach (var v in players.Keys)
                            players[v].IsEnable = false;
                    btnOK.interactable = false;
                    btnCancel.interactable = false;
                    break;
                case MessageType.ShowArgsUI:
                    if (GameManager.UIInstance.PeekWindow() != Framework.UI.WindowType.ChooseArgsUI)
                        GameManager.UIInstance.PushWindow(Framework.UI.WindowType.ChooseArgsUI,
                            Framework.UI.WinMsg.None, Vector3.zero, parameters[0], "Energy");
                    break;
                case MessageType.CloseArgsUI:
                    if (GameManager.UIInstance.PeekWindow() == Framework.UI.WindowType.ChooseArgsUI)
                        GameManager.UIInstance.PopWindow(Framework.UI.WinMsg.None);
                    break;
            }
        }

        public void OnBtnBuyClick()
        {
            BattleData.Instance.Agent.FSM.HandleMessage(UIStateMsg.ClickBtn, "Buy");
        }

        public void OnBtnExtractClick()
        {
            BattleData.Instance.Agent.FSM.HandleMessage(UIStateMsg.ClickBtn, "Extract");
        }

        public void OnBtnSynthetizeClick()
        {
            BattleData.Instance.Agent.FSM.HandleMessage(UIStateMsg.ClickBtn, "Syntheis");
        }

        private void onBtnOKClick()
        {
            BattleData.Instance.Agent.PlayerRole.OKAction();
            BattleData.Instance.Agent.PlayerRole.OKAction = null;
            BattleData.Instance.Agent.PlayerRole.CancelAction = null;
        }

        private void onBtnCancelClick()
        {
            BattleData.Instance.Agent.PlayerRole.CancelAction();
            BattleData.Instance.Agent.PlayerRole.OKAction = null;
            BattleData.Instance.Agent.PlayerRole.CancelAction = null;
        }

    }
}


