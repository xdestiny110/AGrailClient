using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using Framework.Message;
using System;
using System.Collections.Generic;
using Framework.AssetBundle;

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
        private Button BtnResign;
        [SerializeField]
        private Button btnSpecial;
        [SerializeField]
        private Button btnCovered;

        private List<PlayerStatusMobile> players;
        private List<SkillUI> skillUIs = new List<SkillUI>();
        private List<CardUI> cardUIs = new List<CardUI>();

        private bool isShowCovered = false;

        void Awake()
        {
            players = GetComponent<BattleUIMobile>().PlayerStatus;

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
                btnOK.interactable = true;
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
                    onUIStateChange();
                    break;
                case MessageType.AgentUpdate:
                    btnOK.interactable = false;
                    btnOK.onClick.RemoveAllListeners();
                    btnOK.onClick.AddListener(onBtnOKClick);
                    btnCancel.onClick.RemoveAllListeners();
                    btnCancel.onClick.AddListener(onBtnCancelClick);
                    BtnResign.onClick.RemoveAllListeners();
                    BtnResign.onClick.AddListener(onBtnResignClick);
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
                    btnSpecial.interactable = BattleData.Instance.Agent.PlayerRole.CheckBuy(BattleData.Instance.Agent.FSM.Current.StateNumber) ||
                        BattleData.Instance.Agent.PlayerRole.CheckExtract(BattleData.Instance.Agent.FSM.Current.StateNumber) ||
                        BattleData.Instance.Agent.PlayerRole.CheckSynthetize(BattleData.Instance.Agent.FSM.Current.StateNumber);
                    btnCovered.interactable = BattleData.Instance.Agent.PlayerRole.HasCoverd;
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
                    foreach (var v in cardUIs)
                            v.IsEnable = false;
                    foreach (var v in skillUIs)
                            v.IsEnable = false;
                    foreach (var v in players)
                            v.IsEnable = false;
                    btnOK.interactable = false;
                    btnCancel.interactable = false;
                    break;
                case MessageType.ShowArgsUI:
                    if (GameManager.UIInstance.PeekWindow() != Framework.UI.WindowType.ChooseArgsUI)
                        GameManager.UIInstance.PushWindow(Framework.UI.WindowType.ChooseArgsUI,
                            Framework.UI.WinMsg.None, Vector3.zero, parameters);
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

        public void OnCoveredClick()
        {
            isShowCovered = !isShowCovered;
            onCoveredClick();
            onUIStateChange();
        }

        private void onCoveredClick()
        {            
            updateAgentCards();
            btnCovered.GetComponentInChildren<Text>().text =
                isShowCovered ? "显示手牌" : "显示盖牌";
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

        private void updateAgentCards()
        {
            for (int i = 0; i < handArea.childCount; i++)
                Destroy(handArea.GetChild(i).gameObject);
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
                if (BattleData.Instance.Agent.PlayerRole.CanSelect(BattleData.Instance.Agent.FSM.Current.StateNumber, BattleData.Instance.PlayerInfos[BattleData.Instance.PlayerIdxOrder[i]]))
                    players[i].IsEnable = true;
                else
                    players[i].IsEnable = false;
            }
            btnOK.interactable = BattleData.Instance.Agent.PlayerRole.CheckOK(BattleData.Instance.Agent.FSM.Current.StateNumber,
                BattleData.Instance.Agent.SelectCards, BattleData.Instance.Agent.SelectPlayers, BattleData.Instance.Agent.SelectSkill);
            btnCancel.interactable = BattleData.Instance.Agent.PlayerRole.CheckCancel(BattleData.Instance.Agent.FSM.Current.StateNumber,
                BattleData.Instance.Agent.SelectCards, BattleData.Instance.Agent.SelectPlayers, BattleData.Instance.Agent.SelectSkill);
            BtnResign.interactable = BattleData.Instance.Agent.PlayerRole.CheckResign(BattleData.Instance.Agent.FSM.Current.StateNumber);
            btnSpecial.interactable = BattleData.Instance.Agent.PlayerRole.CheckBuy(BattleData.Instance.Agent.FSM.Current.StateNumber) ||
                BattleData.Instance.Agent.PlayerRole.CheckExtract(BattleData.Instance.Agent.FSM.Current.StateNumber) ||
                BattleData.Instance.Agent.PlayerRole.CheckSynthetize(BattleData.Instance.Agent.FSM.Current.StateNumber);
            btnCovered.interactable = BattleData.Instance.Agent.PlayerRole.HasCoverd;
            MessageSystem<MessageType>.Notify(MessageType.AgentSelectPlayer);
            MessageSystem<MessageType>.Notify(MessageType.AgentSelectCard);
            MessageSystem<MessageType>.Notify(MessageType.AgentSelectSkill);
        }
    }
}


