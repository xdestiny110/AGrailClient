﻿using UnityEngine;
using UnityEngine.UI;
using Framework.Message;
using System.Collections.Generic;
using Framework.AssetBundle;
using DG.Tweening;

namespace AGrail
{
    public class PlayerAgentUI : MonoBehaviour, IMessageListener<MessageType>
    {
        [SerializeField]
        private Transform handArea;
        [SerializeField]
        private Transform chooseArea;
        [SerializeField]
        private Transform skillArea;
        [SerializeField]
        private Button btnOK;
        [SerializeField]
        private Button btnCancel;
        [SerializeField]
        private Button btnResign;
        [SerializeField]
        private Button btnSpecial;
        [SerializeField]
        private Button btnSetting;
        [SerializeField]
        private Button btnCovered;

        private List<PlayerStatusMobile> players;
        private List<SkillUI> skillUIs = new List<SkillUI>();
        private List<CardUI> cardUIs = new List<CardUI>();

        private bool isShowCovered = false;

        void Awake()
        {
            players = GetComponent<BattleUIMobile>().PlayerStatus;
            btnSetting.onClick.AddListener(onBtnSettingClick);
            btnSpecial.onClick.AddListener(onBtnSpecialClick);
            btnOK.onClick.AddListener(onBtnOKClick);
            btnCancel.onClick.AddListener(onBtnCancelClick);
            btnResign.onClick.AddListener(onBtnResignClick);
            btnCovered.onClick.AddListener(OnCoveredClick);

            MessageSystem<MessageType>.Regist(MessageType.AgentUpdate, this);
            MessageSystem<MessageType>.Regist(MessageType.AgentHandChange, this);
            MessageSystem<MessageType>.Regist(MessageType.AgentStateChange, this);
            MessageSystem<MessageType>.Regist(MessageType.AgentSelectSkill, this);
            MessageSystem<MessageType>.Regist(MessageType.AgentUIStateChange, this);
            MessageSystem<MessageType>.Regist(MessageType.ShowNewArgsUI, this);
            MessageSystem<MessageType>.Regist(MessageType.CloseNewArgsUI, this);
            MessageSystem<MessageType>.Regist(MessageType.Lose, this);
            MessageSystem<MessageType>.Regist(MessageType.Win, this);
            MessageSystem<MessageType>.Regist(MessageType.AgentSelectCard, this);
            MessageSystem<MessageType>.Notify(MessageType.AgentUpdate);
            MessageSystem<MessageType>.Notify(MessageType.AgentHandChange);
            MessageSystem<MessageType>.Notify(MessageType.AgentUIStateChange);
        }

        void OnDestroy()
        {
            MessageSystem<MessageType>.UnRegist(MessageType.AgentUpdate, this);
            MessageSystem<MessageType>.UnRegist(MessageType.AgentHandChange, this);
            MessageSystem<MessageType>.UnRegist(MessageType.AgentStateChange, this);
            MessageSystem<MessageType>.UnRegist(MessageType.AgentSelectSkill, this);
            MessageSystem<MessageType>.UnRegist(MessageType.AgentUIStateChange, this);
            MessageSystem<MessageType>.UnRegist(MessageType.ShowNewArgsUI, this);
            MessageSystem<MessageType>.UnRegist(MessageType.CloseNewArgsUI, this);
            MessageSystem<MessageType>.UnRegist(MessageType.Lose, this);
            MessageSystem<MessageType>.UnRegist(MessageType.Win, this);
            MessageSystem<MessageType>.UnRegist(MessageType.AgentSelectCard, this);
        }


        public void OnEventTrigger(MessageType eventType, params object[] parameters)
        {
            switch (eventType)
            {
                case MessageType.AgentSelectCard:
                    var cards = BattleData.Instance.Agent.SelectCards;
                    if (cards.Count > 1)
                    {
                        chooseArea.gameObject.SetActive(true);
                        foreach (var v in cardUIs)
                        {
                            if (cards.Contains(v.Card.ID))
                                v.transform.SetParent(chooseArea);
                            else
                                v.transform.SetParent(handArea);
                        }
                    }
                    else
                    {
                        chooseArea.gameObject.SetActive(false);
                        foreach (var v in cardUIs)
                            v.transform.SetParent(handArea);
                    }
                    break;
                case MessageType.AgentUIStateChange:
                    onUIStateChange();
                    break;
                case MessageType.AgentUpdate:
                    //初始化技能键
                    var skillPrefab = AssetBundleManager.Instance.LoadAsset("battle", "Skill");
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
                    if (parameters.Length == 1)
                    {
                        if((bool)parameters[0] != isShowCovered)
                        {
                            isShowCovered = (bool)parameters[0];
                            onCoveredClick();
                        }
                    }
                    else
                        updateAgentCards();
                    break;
                case MessageType.AgentStateChange:
                    //保证在初始状态
                    //foreach (var v in cardUIs)
                    //        v.IsEnable = false;
                    //foreach (var v in skillUIs)
                    //        v.IsEnable = false;
                    //foreach (var v in players)
                    //        v.IsEnable = false;
                    //btnOK.gameObject.SetActive(false);
                    //btnCancel.gameObject.SetActive(false);
                    //while (GameManager.UIInstance.PeekWindow() != Framework.UI.WindowType.BattleUIMobile)
                    //    GameManager.UIInstance.PopWindow(Framework.UI.WinMsg.None);
                    break;
                case MessageType.ShowNewArgsUI:
                    if (GameManager.UIInstance.PeekWindowType() != Framework.UI.WindowType.ArgsUI)
                    { 
                        if (GameManager.UIInstance.PeekWindowType() == Framework.UI.WindowType.InfomationUI || 
                            GameManager.UIInstance.PeekWindowType() == Framework.UI.WindowType.OptionsUI)
                            GameManager.UIInstance.PopWindow(Framework.UI.WinMsg.None);
                        GameManager.UIInstance.PushWindow(Framework.UI.WindowType.ArgsUI, Framework.UI.WinMsg.None, -1, Vector3.zero, parameters);
                    }
                    break;
                case MessageType.CloseNewArgsUI:
                    if (GameManager.UIInstance.PeekWindowType() == Framework.UI.WindowType.ArgsUI)
                        GameManager.UIInstance.PopWindow(Framework.UI.WinMsg.None);
                    break;
            }
        }
        public void OnCoveredClick()
        {
            isShowCovered = !isShowCovered;
            onCoveredClick();
            onUIStateChange();
        }

        private void onCoveredClick()
        {
            updateAgentCards();
            //btnCovered.GetComponentInChildren<Text>().text =
            //    isShowCovered ? "显示手牌" : "显示盖牌";
        }

        private void onBtnOKClick()
        {
            BattleData.Instance.Agent.PlayerRole.OKAction();
        }

        private void onBtnCancelClick()
        {
            BattleData.Instance.Agent.PlayerRole.CancelAction();
        }

        private void onBtnResignClick()
        {
            BattleData.Instance.Agent.PlayerRole.ResignAction();
        }

        private void onBtnSpecialClick()
        {
            GameManager.UIInstance.PushWindow(Framework.UI.WindowType.SpecialActionUI, Framework.UI.WinMsg.None);
        }

        private void onBtnSettingClick()
        {
            GameManager.UIInstance.PushWindow(Framework.UI.WindowType.OptionsUI, Framework.UI.WinMsg.None);
        }

        private void updateAgentCards()
        {
            for (int i = 0; i < cardUIs.Count; i++)
                Destroy(cardUIs[i].gameObject);
            cardUIs.Clear();
            List<uint> cards = isShowCovered ? BattleData.Instance.MainPlayer.covereds : BattleData.Instance.MainPlayer.hands;
            var cardPrefab = AssetBundleManager.Instance.LoadAsset("battle", "Card");
            foreach (var v in cards)
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
        }

        private void onUIStateChange()
        {
            //UI状态变化，确认哪些能够选择
            var fsm = BattleData.Instance.Agent.FSM.Current;
            foreach (var v in cardUIs)
            {
                if (BattleData.Instance.Agent.PlayerRole.CanSelect(BattleData.Instance.Agent.FSM.Current.StateNumber, v.Card, isShowCovered))
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
            for (int i = 0; i < players.Count; i++)
            {
                //var fsm = BattleData.Instance.Agent.FSM.Current;
                if (fsm != null)
                {
                    if (BattleData.Instance.Agent.PlayerRole.CanSelect(fsm.StateNumber, BattleData.Instance.GetPlayerInfo((uint)BattleData.Instance.PlayerIdxOrder[i])))
                        players[i].IsEnable = true;
                    else
                        players[i].IsEnable = false;
                }
            }
            
            if (fsm != null)
            {
                btnOK.gameObject.SetActive(BattleData.Instance.Agent.PlayerRole.CheckOK(fsm.StateNumber,
                    BattleData.Instance.Agent.SelectCards, BattleData.Instance.Agent.SelectPlayers, BattleData.Instance.Agent.SelectSkill));
                btnCancel.gameObject.SetActive(BattleData.Instance.Agent.PlayerRole.CheckCancel(fsm.StateNumber,
                    BattleData.Instance.Agent.SelectCards, BattleData.Instance.Agent.SelectPlayers, BattleData.Instance.Agent.SelectSkill));
                btnResign.gameObject.SetActive(BattleData.Instance.Agent.PlayerRole.CheckResign(fsm.StateNumber));
                btnSpecial.gameObject.SetActive(BattleData.Instance.Agent.PlayerRole.CheckBuy(fsm.StateNumber) ||
                    BattleData.Instance.Agent.PlayerRole.CheckExtract(fsm.StateNumber) ||
                    BattleData.Instance.Agent.PlayerRole.CheckSynthetize(fsm.StateNumber));
            }
            btnCovered.gameObject.SetActive(BattleData.Instance.Agent.PlayerRole.HasCoverd);
            MessageSystem<MessageType>.Notify(MessageType.AgentSelectPlayer);
            MessageSystem<MessageType>.Notify(MessageType.AgentSelectCard);
            MessageSystem<MessageType>.Notify(MessageType.AgentSelectSkill);
        }
    }
}


