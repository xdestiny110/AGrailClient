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
            MessageSystem<MessageType>.Regist(MessageType.AgentSetOKCallback, this);
            MessageSystem<MessageType>.Regist(MessageType.AgentSetCancelCallback, this);
            MessageSystem<MessageType>.Regist(MessageType.AgentHandChange, this);
            MessageSystem<MessageType>.Regist(MessageType.AgentStateChange, this);
            MessageSystem<MessageType>.Regist(MessageType.AgentSelectSkill, this);
            MessageSystem<MessageType>.Regist(MessageType.AgentUIStateChange, this);

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
            MessageSystem<MessageType>.UnRegist(MessageType.AgentUIStateChange, this);
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
            }
        }

        public void OnBtnBuyClick()
        {
            if (BattleData.Instance.Gem[BattleData.Instance.MainPlayer.team] + BattleData.Instance.Crystal[BattleData.Instance.MainPlayer.team] == 4)
            {
                var selectList = new List<List<uint>>();
                selectList.Add(new List<uint>() { 1, 0 });
                selectList.Add(new List<uint>() { 0, 1 });
                BattleData.Instance.Agent.FSM.HandleMessage(UIStateMsg.ClickBtn, "Buy");
                GameManager.UIInstance.PushWindow(Framework.UI.WindowType.ChooseArgsUI, Framework.UI.WinMsg.None, Vector3.zero,
                    "Energy", selectList);
            }
            else
            {
                BattleData.Instance.Agent.PlayerRole.Buy(1, 1);
                BattleData.Instance.Agent.FSM.ChangeState<StateIdle>(UIStateMsg.Init, true);
            }                
        }

        public void OnBtnExtractClick()
        {
            //BattleData.Instance.Agent.AgentUIState = 13;
            //var selectList = new List<List<uint>>();
            //if (BattleData.Instance.Gem[BattleData.Instance.MainPlayer.team] >= 2)
            //    selectList.Add(new List<uint>() { 2, 0 });
            //if (BattleData.Instance.Crystal[BattleData.Instance.MainPlayer.team] >= 2)
            //    selectList.Add(new List<uint>() { 0, 2 });
            //if (BattleData.Instance.Gem[BattleData.Instance.MainPlayer.team] >= 1 && BattleData.Instance.Crystal[BattleData.Instance.MainPlayer.team] >= 1)
            //    selectList.Add(new List<uint>() { 1, 1 });
            //if (BattleData.Instance.Gem[BattleData.Instance.MainPlayer.team] >= 1)
            //    selectList.Add(new List<uint>() { 1, 0 });
            //if (BattleData.Instance.Crystal[BattleData.Instance.MainPlayer.team] >= 1)
            //    selectList.Add(new List<uint>() { 0, 1 });
            //GameManager.UIInstance.PushWindow(Framework.UI.WindowType.ChooseArgsUI, Framework.UI.WinMsg.None, Vector3.zero,
            //    "Energy", selectList);
        }

        public void OnBtnSynthetizeClick()
        {
            //BattleData.Instance.Agent.AgentUIState = 14;
            //var selectList = new List<List<uint>>();
            //if (BattleData.Instance.Crystal[BattleData.Instance.MainPlayer.team] >= 3)
            //    selectList.Add(new List<uint>() { 0, 3 });
            //if (BattleData.Instance.Gem[BattleData.Instance.MainPlayer.team] >= 1 && BattleData.Instance.Crystal[BattleData.Instance.MainPlayer.team] >= 2)
            //    selectList.Add(new List<uint>() { 1, 2 });
            //if (BattleData.Instance.Gem[BattleData.Instance.MainPlayer.team] >= 2 && BattleData.Instance.Crystal[BattleData.Instance.MainPlayer.team] >= 1)
            //    selectList.Add(new List<uint>() { 2, 1 });
            //if (BattleData.Instance.Gem[BattleData.Instance.MainPlayer.team] >= 3)
            //    selectList.Add(new List<uint>() { 3, 0 });
            //GameManager.UIInstance.PushWindow(Framework.UI.WindowType.ChooseArgsUI, Framework.UI.WinMsg.None, Vector3.zero,
            //    "Energy", selectList);
        }

        private void onBtnOKClick()
        {
            switch (BattleData.Instance.Agent.FSM.Current.StateNumber)
            {
                case 1:
                    //攻击
                    BattleData.Instance.Agent.PlayerRole.Attack(BattleData.Instance.Agent.SelectCards[0],
                        BattleData.Instance.Agent.SelectPlayers[0]);
                    break;
                case 2:
                    //魔法
                    BattleData.Instance.Agent.PlayerRole.Magic(BattleData.Instance.Agent.SelectCards[0],
                        BattleData.Instance.Agent.SelectPlayers[0]);
                    break;
                case 3:
                    //应战
                    if (BattleData.Instance.Agent.SelectCards.Count > 0)
                    {
                        if (BattleData.Instance.Agent.SelectPlayers.Count > 0)
                            BattleData.Instance.Agent.PlayerRole.AttackedReply(
                                Card.GetCard(BattleData.Instance.Agent.SelectCards[0]),
                                BattleData.Instance.Agent.SelectPlayers[0]);
                        else
                            BattleData.Instance.Agent.PlayerRole.AttackedReply(
                                Card.GetCard(BattleData.Instance.Agent.SelectCards[0]));
                    }
                    break;
                case 4:
                    //魔弹响应
                    BattleData.Instance.Agent.PlayerRole.MoDaned(Card.GetCard(BattleData.Instance.Agent.SelectCards[0]));
                    break;
                case 5:
                    //弃牌
                    BattleData.Instance.Agent.PlayerRole.Drop(BattleData.Instance.Agent.SelectCards);
                    break;
                case 6:
                    //虚弱
                    BattleData.Instance.Agent.PlayerRole.Weaken(new List<uint>() { 1 });
                    break;
                case 7:
                    //治疗
                    break;
                case 10:
                    //最通常的回合开始的状态
                    break;
                case 11:
                    //只能攻击或法术
                    break;
                case 13:
                    //选择参数
                    GameManager.UIInstance.PopWindow(Framework.UI.WinMsg.None);
                    BattleData.Instance.Agent.FSM.HandleMessage(UIStateMsg.ClickBtn, "OK");
                    onBtnOKClick();
                    break;
                case 42:
                    //额外行动
                    //BattleData.Instance.Agent.PlayerRole.AdditionAction();
                    break;
                default:
                    //技能
                    BattleData.Instance.Agent.PlayerRole.UseSkill(true);
                    break;
            }
            BattleData.Instance.Agent.FSM.ChangeState<StateIdle>(UIStateMsg.Init, true);
        }

        private void onBtnCancelClick()
        {
            switch (BattleData.Instance.Agent.FSM.Current.StateNumber)
            {
                case 3:
                    //应战
                    BattleData.Instance.Agent.PlayerRole.AttackedReply();
                    break;
                case 4:
                    //魔弹响应
                    BattleData.Instance.Agent.PlayerRole.MoDaned();
                    break;
                case 6:
                    //虚弱
                    BattleData.Instance.Agent.PlayerRole.Weaken(new List<uint>() { 0 });
                    break;
                case 7:
                    //治疗
                    break;
                case 13:
                    //选择参数
                    GameManager.UIInstance.PopWindow(Framework.UI.WinMsg.None);
                    BattleData.Instance.Agent.FSM.BackState(UIStateMsg.ClickBtn);
                    break;
                case 42:
                    //额外行动
                    //BattleData.Instance.Agent.PlayerRole.AdditionAction();
                    break;
                default:
                    //技能
                    BattleData.Instance.Agent.PlayerRole.UseSkill(false);
                    break;
            }
            BattleData.Instance.Agent.FSM.ChangeState<StateIdle>(UIStateMsg.Init, true);
        }

    }
}


