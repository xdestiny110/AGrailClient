using Framework.UI;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Framework.Message;
using Framework.AssetBundle;
using System.Linq;
using DG.Tweening;
using UnityEngine.SceneManagement;

namespace AGrail
{
    public class BattleUIMobile : WindowsBase
    {
        [SerializeField]
        private Transform root;
        [SerializeField]
        private Text[] morales;
        [SerializeField]
        private Transform[] energies;
        [SerializeField]
        private Transform[] grails;
        [SerializeField]
        public List<PlayerStatusMobile> playerStatus;
        [SerializeField]
        private Transform showCardArea;
        [SerializeField]
        private Text hint;
        [SerializeField]
        private Text turn;
        [SerializeField]
        private Transform logRoot;
        [SerializeField]
        private Text log;
        [SerializeField]
        private Transform chatRoot;
        [SerializeField]
        private Transform chat;
        [SerializeField]
        private Button btnLogExpand;
        [SerializeField]
        private Button btnChatExpand;
        [SerializeField]
        private InputField inptChat;
        [SerializeField]
        private Button btnSubmit;
        [SerializeField]
        private GameObject winPanel;
        [SerializeField]
        private Text endText;
        [SerializeField]
        private Image winImage;
        [SerializeField]
        private Transform MVProot;
        [SerializeField]
        public List<PlayerIco> MVPS;

        public List<PlayerStatusMobile> PlayerStatus
        {
            get
            {
                return playerStatus;
            }
        }

        public override WindowType Type
        {
            get
            {
                return WindowType.BattleUIMobile;
            }
        }

        public override void Awake()
        {
            if (Lobby.Instance.SelectRoom.max_player != 6)
            {
                GameObject.Destroy(playerStatus[2].gameObject);
                playerStatus.RemoveAt(2);
                GameObject.Destroy(playerStatus[3].gameObject);
                playerStatus.RemoveAt(3);
                GameObject.Destroy(MVPS[0].gameObject);
                MVPS.RemoveAt(0);
                GameObject.Destroy(MVPS[1].gameObject);
                MVPS.RemoveAt(1);
            }
            Dialog.Instance.Reset();

            btnLogExpand.onClick.AddListener(delegate { onBtnLogExpandClick(true); });
            btnChatExpand.onClick.AddListener(delegate { onBtnChatExpandClick(true); });
            btnSubmit.onClick.AddListener(delegate { onBtnSubmitClick(null); });
            inptChat.onEndEdit.AddListener(onBtnSubmitClick);

            MessageSystem<MessageType>.Regist(MessageType.MoraleChange, this);
            MessageSystem<MessageType>.Regist(MessageType.GemChange, this);
            MessageSystem<MessageType>.Regist(MessageType.CrystalChange, this);
            MessageSystem<MessageType>.Regist(MessageType.GrailChange, this);
            MessageSystem<MessageType>.Regist(MessageType.SendHint, this);
            MessageSystem<MessageType>.Regist(MessageType.Win, this);
            MessageSystem<MessageType>.Regist(MessageType.Lose, this);
            MessageSystem<MessageType>.Regist(MessageType.PlayerNickName, this);
            MessageSystem<MessageType>.Regist(MessageType.PlayerActionChange, this);
            MessageSystem<MessageType>.Regist(MessageType.PlayerTeamChange, this);
            MessageSystem<MessageType>.Regist(MessageType.PlayerRoleChange, this);
            MessageSystem<MessageType>.Regist(MessageType.PlayerHandChange, this);
            MessageSystem<MessageType>.Regist(MessageType.PlayerHealChange, this);
            MessageSystem<MessageType>.Regist(MessageType.PlayerTokenChange, this);
            MessageSystem<MessageType>.Regist(MessageType.PlayerKneltChange, this);
            MessageSystem<MessageType>.Regist(MessageType.PlayerEnergeChange, this);
            MessageSystem<MessageType>.Regist(MessageType.PlayerBasicAndExCardChange, this);
            MessageSystem<MessageType>.Regist(MessageType.HURTMSG, this);
            MessageSystem<MessageType>.Regist(MessageType.CARDMSG, this);
            MessageSystem<MessageType>.Regist(MessageType.SKILLMSG, this);
            MessageSystem<MessageType>.Regist(MessageType.TURNBEGIN, this);
            MessageSystem<MessageType>.Regist(MessageType.LogChange, this);
            MessageSystem<MessageType>.Regist(MessageType.ChatChange, this);
            MessageSystem<MessageType>.Regist(MessageType.POLLINGREQUEST, this);

            //依据数据初始化界面
            MessageSystem<MessageType>.Notify(MessageType.PlayBGM);
            MessageSystem<MessageType>.Notify(MessageType.MoraleChange, Team.Red, BattleData.Instance.Morale[(int)Team.Red]);
            MessageSystem<MessageType>.Notify(MessageType.MoraleChange, Team.Blue, BattleData.Instance.Morale[(int)Team.Blue]);
            MessageSystem<MessageType>.Notify(MessageType.GemChange, Team.Red, (int)BattleData.Instance.Gem[(int)Team.Red]);
            MessageSystem<MessageType>.Notify(MessageType.GemChange, Team.Blue, (int)BattleData.Instance.Gem[(int)Team.Blue]);
            MessageSystem<MessageType>.Notify(MessageType.CrystalChange, Team.Red, (int)BattleData.Instance.Crystal[(int)Team.Red]);
            MessageSystem<MessageType>.Notify(MessageType.CrystalChange, Team.Blue, (int)BattleData.Instance.Crystal[(int)Team.Blue]);
            MessageSystem<MessageType>.Notify(MessageType.GrailChange, Team.Red, BattleData.Instance.Grail[(int)Team.Red]);
            MessageSystem<MessageType>.Notify(MessageType.GrailChange, Team.Blue, BattleData.Instance.Grail[(int)Team.Blue]);

            var playerIdxOrder = BattleData.Instance.PlayerIdxOrder;
            if(playerIdxOrder.Count == BattleData.Instance.PlayerInfos.Count && playerIdxOrder.Count > 0)
            {
                for (int i = 0; i < playerStatus.Count; i++)
                {
                    var playerInfo = BattleData.Instance.GetPlayerInfo((uint)playerIdxOrder[i]);
                    playerStatus[i].ID = playerInfo.id;
                    MessageSystem<MessageType>.Notify(MessageType.PlayerNickName, i, playerInfo.nickname);
                    MessageSystem<MessageType>.Notify(MessageType.PlayerTeamChange, i, playerInfo.team);
                    MessageSystem<MessageType>.Notify(MessageType.PlayerRoleChange, i, playerInfo.role_id);
                    MessageSystem<MessageType>.Notify(MessageType.PlayerHandChange, i, playerInfo.hand_count, playerInfo.max_hand);
                    MessageSystem<MessageType>.Notify(MessageType.PlayerHealChange, i, playerInfo.heal_count);
                    MessageSystem<MessageType>.Notify(MessageType.PlayerTokenChange, i,
                        playerInfo.yellow_token, playerInfo.blue_token, playerInfo.covered_count);
                    MessageSystem<MessageType>.Notify(MessageType.PlayerEnergeChange, i, playerInfo.gem, playerInfo.crystal);
                    MessageSystem<MessageType>.Notify(MessageType.PlayerKneltChange, i, playerInfo.is_knelt);
                    MessageSystem<MessageType>.Notify(MessageType.PlayerBasicAndExCardChange, i, playerInfo.basic_cards, playerInfo.ex_cards);
                }
            }

            base.Awake();
        }

        public override void OnDestroy()
        {
            MessageSystem<MessageType>.UnRegist(MessageType.MoraleChange, this);
            MessageSystem<MessageType>.UnRegist(MessageType.GemChange, this);
            MessageSystem<MessageType>.UnRegist(MessageType.CrystalChange, this);
            MessageSystem<MessageType>.UnRegist(MessageType.GrailChange, this);
            MessageSystem<MessageType>.UnRegist(MessageType.SendHint, this);
            MessageSystem<MessageType>.UnRegist(MessageType.Win, this);
            MessageSystem<MessageType>.UnRegist(MessageType.Lose, this);
            MessageSystem<MessageType>.UnRegist(MessageType.PlayerNickName, this);
            MessageSystem<MessageType>.UnRegist(MessageType.PlayerActionChange, this);
            MessageSystem<MessageType>.UnRegist(MessageType.PlayerTeamChange, this);
            MessageSystem<MessageType>.UnRegist(MessageType.PlayerRoleChange, this);
            MessageSystem<MessageType>.UnRegist(MessageType.PlayerHandChange, this);
            MessageSystem<MessageType>.UnRegist(MessageType.PlayerHealChange, this);
            MessageSystem<MessageType>.UnRegist(MessageType.PlayerTokenChange, this);
            MessageSystem<MessageType>.UnRegist(MessageType.PlayerKneltChange, this);
            MessageSystem<MessageType>.UnRegist(MessageType.PlayerEnergeChange, this);
            MessageSystem<MessageType>.UnRegist(MessageType.PlayerBasicAndExCardChange, this);
            MessageSystem<MessageType>.UnRegist(MessageType.HURTMSG, this);
            MessageSystem<MessageType>.UnRegist(MessageType.CARDMSG, this);
            MessageSystem<MessageType>.UnRegist(MessageType.SKILLMSG, this);
            MessageSystem<MessageType>.UnRegist(MessageType.TURNBEGIN, this);
            MessageSystem<MessageType>.UnRegist(MessageType.LogChange, this);
            MessageSystem<MessageType>.UnRegist(MessageType.ChatChange, this);
            MessageSystem<MessageType>.UnRegist(MessageType.POLLINGREQUEST, this);

            base.OnDestroy();
        }

        public override void OnEventTrigger(MessageType eventType, params object[] parameters)
        {
            switch (eventType)
            {
                case MessageType.MoraleChange:
                    morales[(int)parameters[0]].text = parameters[1].ToString();
                    break;
                case MessageType.GemChange:
                    gemChange((Team)parameters[0], (int)parameters[1]);
                    break;
                case MessageType.CrystalChange:
                    crystalChange((Team)parameters[0], (int)parameters[1]);
                    break;
                case MessageType.GrailChange:
                    grailChange((Team)parameters[0], (uint)parameters[1]);
                    break;
                case MessageType.SendHint:
                    if (IsInvoking("hideHint"))
                        CancelInvoke("hideHint");
                    if (parameters.Length != 1)
                        Invoke("hideHint", 3.0f);
                    else
                    {
                        hint.transform.parent.gameObject.SetActive(true);
                        hint.text = parameters[0].ToString();
                    }
                    break;
                case MessageType.Win:
                case MessageType.Lose:
                    for (int i=0;i < MVPS.Count;i++)
                    {
                        MVPS[i].IcoID = (uint)i;
                        MVPS[i].canselect = true;
                        int a = i;
                        MVPS[i].MVP.onClick.AddListener(delegate { onMVPClick(a); });
                    }
                    winPanel.SetActive(true);
                    winImage.sprite =
                        (
                        (BattleData.Instance.Morale[0] == 0 || BattleData.Instance.Grail[1]==5 ) ?
                        AssetBundleManager.Instance.LoadAsset<Sprite>("battle_texture", "WinRed") :
                        AssetBundleManager.Instance.LoadAsset<Sprite>("battle_texture", "WinBlue")
                        );
                    break;
                case MessageType.PlayerNickName:
                    playerStatus[(int)parameters[0]].NickName = parameters[1].ToString();
                    break;
                case MessageType.PlayerTeamChange:
                    var playerInfo = BattleData.Instance.GetPlayerInfo((uint)BattleData.Instance.PlayerIdxOrder[(int)parameters[0]]);
                    playerStatus[(int)parameters[0]].ID = playerInfo.id;
                    playerStatus[(int)parameters[0]].Team = (Team)(uint)parameters[1];
                    break;
                case MessageType.PlayerRoleChange:
                    playerStatus[(int)parameters[0]].RoleID = (uint)parameters[1];
                    break;
                case MessageType.PlayerTokenChange:
                    playerStatus[(int)parameters[0]].YellowToken = (uint)parameters[1];
                    playerStatus[(int)parameters[0]].BlueToken = (uint)parameters[2];
                    playerStatus[(int)parameters[0]].Covered = (uint)parameters[3];
                    break;
                case MessageType.PlayerKneltChange:
                    playerStatus[(int)parameters[0]].Knelt = (bool)parameters[1];
                    break;
                case MessageType.PlayerBasicAndExCardChange:
                    playerStatus[(int)parameters[0]].BasicAndExCards = (parameters[1] as List<uint>).Union(parameters[2] as List<uint>).ToList();
                    break;
                case MessageType.PlayerEnergeChange:
                    playerStatus[(int)parameters[0]].Energy = new KeyValuePair<uint, uint>((uint)parameters[1], (uint)parameters[2]);
                    break;
                case MessageType.PlayerHandChange:
                    playerStatus[(int)parameters[0]].HandCount = new KeyValuePair<uint, uint>((uint)parameters[1], (uint)parameters[2]);
                    break;
                case MessageType.PlayerHealChange:
                    playerStatus[(int)parameters[0]].HealCount = (uint)parameters[1];
                    break;
                case MessageType.HURTMSG:
                    var hurtMsg = parameters[0] as network.HurtMsg;
                    if (hurtMsg.dst_idSpecified)
                        playerStatus[BattleData.Instance.PlayerIdxOrder.IndexOf((int)hurtMsg.dst_id)].Hurted();
                    break;
                case MessageType.CARDMSG:
                    var cardMsg = parameters[0] as network.CardMsg;
                    if ((cardMsg.is_realSpecified && cardMsg.is_real && cardMsg.type == 1) || cardMsg.type == 2)
                        showCard(cardMsg.card_ids);
                    if (cardMsg.typeSpecified && cardMsg.type == 1 && cardMsg.dst_id != cardMsg.src_id)
                        actionAnim(cardMsg.src_id, cardMsg.dst_id);
                    break;
                case MessageType.TURNBEGIN:
                    var tb = parameters[0] as network.TurnBegin;
                    if (tb.roundSpecified)
                        turn.text = tb.round.ToString();
                    if (tb.idSpecified)
                    {
                        for (int i = 0; i < playerStatus.Count; i++)
                        {
                            if (BattleData.Instance.GetPlayerInfo((uint)BattleData.Instance.PlayerIdxOrder[i]).id == tb.id)
                                PlayerStatus[i].TurnBegin();
                            else
                                playerStatus[i].Turn = false;
                        }
                        MessageSystem<MessageType>.Notify(MessageType.SendHint);
                    }
                    break;
                case MessageType.PlayerActionChange:
                    for (int i = 0; i < playerStatus.Count; i++)
                    {
                        if (parameters.Length > 0)
                            playerStatus[i].WaitAction = (playerStatus[i].ID == (uint)parameters[0]) ? parameters[1].ToString() : "";
                        else
                            playerStatus[i].WaitAction = "";
                    }
                    break;
                case MessageType.LogChange:
                    log.text = Dialog.Instance.Log;
                    break;
                case MessageType.ChatChange:
                    chatChange();
                    break;
                case MessageType.POLLINGREQUEST:
                    var pollReq = parameters[0] as network.PollingRequest;
                    if(pollReq.options.Count == 2)
                    {
                        BattleData.Instance.Agent.AgentState = (int)PlayerAgentState.Polling;
                        if (GameManager.UIInstance.PeekWindow() == Framework.UI.WindowType.DisConnectedPoll)
                            GameManager.UIInstance.PopWindow(Framework.UI.WinMsg.None);
                        GameManager.UIInstance.PushWindow(Framework.UI.WindowType.DisConnectedPoll, Framework.UI.WinMsg.None, -1, Vector3.zero);
                    }
                    break;
            }
        }

        private void gemChange(Team team, int diffGem)
        {
            if (diffGem > 0)
            {
                var prefab = AssetBundleManager.Instance.LoadAsset("battle", "Image");
                for (int i = 0; i < diffGem; i++)
                {
                    var go = Instantiate(prefab);
                    go.transform.SetParent(energies[(int)team]);
                    go.transform.localPosition = Vector3.zero;
                    go.transform.localRotation = Quaternion.identity;
                    go.transform.localScale = Vector3.one;
                    go.transform.SetSiblingIndex(0);
                    go.GetComponent<Image>().sprite = AssetBundleManager.Instance.LoadAsset<Sprite>("battle_texture", "gem");
                }
            }
            else
            {
                for (int i = 0; i < Mathf.Abs(diffGem); i++)
                    Destroy(energies[(int)team].GetChild(i).gameObject);
            }
        }

        private void crystalChange(Team team, int diffCrystal)
        {
            if (diffCrystal > 0)
            {
                var prefab = AssetBundleManager.Instance.LoadAsset("battle", "Image");
                for (int i = 0; i < diffCrystal; i++)
                {
                    var go = Instantiate(prefab);
                    go.transform.SetParent(energies[(int)team]);
                    go.transform.localPosition = Vector3.zero;
                    go.transform.localRotation = Quaternion.identity;
                    go.transform.localScale = Vector3.one;
                    go.transform.SetSiblingIndex(energies[(int)team].childCount - 1);
                    go.GetComponent<Image>().sprite = AssetBundleManager.Instance.LoadAsset<Sprite>("battle_texture", "crystal");
                }
            }
            else
            {
                for (int i = energies[(int)team].childCount - 1; i > energies[(int)team].childCount - 1 + diffCrystal; i--)
                    Destroy(energies[(int)team].GetChild(i).gameObject);
            }
        }

        private void grailChange(Team team, uint diffGrail)
        {
            var prefab = AssetBundleManager.Instance.LoadAsset("battle", "Image");
            for (int i = 0; i < diffGrail; i++)
            {
                var go = Instantiate(prefab);
                go.transform.SetParent(grails[(int)team]);
                go.transform.localPosition = Vector3.zero;
                go.transform.localRotation = Quaternion.identity;
                go.transform.localScale = Vector3.one;
                go.GetComponent<Image>().sprite = AssetBundleManager.Instance.LoadAsset<Sprite>("battle_texture", "grail");
            }
        }

        private void showCard(List<uint> card_ids)
        {
            for (int i = 0; i < showCardArea.childCount; i++)
                Destroy(showCardArea.GetChild(i).gameObject);
            foreach (var v in card_ids)
            {
                var card = Card.GetCard(v);
                var go = Instantiate(AssetBundleManager.Instance.LoadAsset("battle", "Card"));
                go.transform.SetParent(showCardArea);
                go.transform.localScale = Vector3.one;
                go.transform.localPosition = Vector3.zero;
                go.transform.localRotation = Quaternion.identity;
                var cardUI = go.GetComponent<CardUI>();
                cardUI.Card = card;
                cardUI.Disappear();
            }
        }

        private void actionAnim(uint src_id, uint dst_id)
        {
            int srcIdx = -1, dstIdx = -1;

            srcIdx = BattleData.Instance.PlayerIdxOrder.IndexOf((int)src_id);
            dstIdx = BattleData.Instance.PlayerIdxOrder.IndexOf((int)dst_id);
            if (srcIdx < 0 || dstIdx < 0)
            {
                Debug.LogErrorFormat("srcIdx = {0}, dstIdx = {1}", srcIdx, dstIdx);
                Debug.LogErrorFormat("srcID = {0}, dstID = {1}", src_id, dst_id);
                return;
            }

            var prefab = AssetBundleManager.Instance.LoadAsset("battle", "Arrow");
            var arrow = Instantiate(prefab);
            arrow.transform.SetParent(root);
            arrow.transform.position = playerStatus[srcIdx].AnimationPos.position;
            arrow.transform.localScale = Vector3.one;
            arrow.GetComponent<Arrow>().SetParms(playerStatus[srcIdx].AnimationPos.position, playerStatus[dstIdx].AnimationPos.position);
            Debug.LogFormat("src pos = {0}, dst pos = {1}", playerStatus[srcIdx].AnimationPos.position, playerStatus[dstIdx].AnimationPos.position);
        }

        private void chatChange()
        {
            var go = Instantiate<GameObject>(AssetBundleManager.Instance.LoadAsset("battle", "ChatItem"));
            go.transform.parent = this.chat;
            go.transform.localScale = Vector3.one;
            go.transform.localPosition = Vector3.zero;
            go.transform.localRotation = Quaternion.identity;
            var chatItem = go.GetComponent<ChatItem>();
            var chat = Dialog.Instance.Chat.Last();
            chatItem.RoleID = chat.RoleID;
            chatItem.Msg = (chat.RoleID == null) ? BattleData.Instance.GetPlayerInfo(chat.ID).nickname + ": " + chat.msg : chat.msg;
            if (chat.ID != BattleData.Instance.PlayerID)
                chatItem.IsMainPlayer = false;

            //生成气泡文字
            playerStatus[BattleData.Instance.PlayerIdxOrder.IndexOf((int)chat.ID)].Chat = chat.msg;
        }

        private Tweener logTweener = null;
        private void onBtnLogExpandClick(bool flag)
        {
            if (logTweener != null && logTweener.IsPlaying())
                return;
            btnLogExpand.onClick.RemoveAllListeners();
            if (flag)
            {
                logRoot.localPosition = new Vector3(960, 0, 0);
                logTweener = logRoot.DOLocalMoveX(320, 0.5f);
                btnLogExpand.onClick.AddListener(delegate { onBtnLogExpandClick(false); });
            }
            else
            {
                logRoot.localPosition = new Vector3(320, 0, 0);
                logTweener = logRoot.DOLocalMoveX(960, 0.5f);
                btnLogExpand.onClick.AddListener(delegate { onBtnLogExpandClick(true); });
            }
        }

        private Tweener chatTweener = null;
        private void onBtnChatExpandClick(bool flag)
        {
            if (chatTweener != null && chatTweener.IsPlaying())
                return;
            btnChatExpand.onClick.RemoveAllListeners();
            if (flag)
            {
                chatRoot.localPosition = new Vector3(-960, 0, 0);
                chatTweener = chatRoot.DOLocalMoveX(-320, 0.5f);
                btnChatExpand.onClick.AddListener(delegate { onBtnChatExpandClick(false); });
            }
            else
            {
                chatRoot.localPosition = new Vector3(-320, 0, 0);
                chatTweener = chatRoot.DOLocalMoveX(-960, 0.5f);
                btnChatExpand.onClick.AddListener(delegate { onBtnChatExpandClick(true); });
            }
        }

        private void onBtnSubmitClick(string str)
        {
            if (!string.IsNullOrEmpty(inptChat.text))
                Dialog.Instance.SendTalk(inptChat.text);
            inptChat.text = string.Empty;
        }

        private void hideHint()
        {
            hint.transform.parent.gameObject.SetActive(false);
        }

        private void onMVPClick(int mvp)
        {
            for (int i = 0; i < MVPS.Count; i++)
            {
                MVPS[i].canselect = false;
                if (i == mvp)
                    MVPS[mvp].Selected = true;
            }
            network.PollingResponse proto = new network.PollingResponse() { option = (uint)mvp };
            GameManager.TCPInstance.Send(new Framework.Network.Protobuf() { Proto = proto, ProtoID = ProtoNameIds.POLLINGRESPONSE });
            endText.text = "你选择了" + BattleData.Instance.GetPlayerInfo((uint)mvp).nickname + "作为MVP,你可以等待结果,也可以选择退出";
        }

        public void winAndExit()
        {
            Lobby.Instance.LeaveRoom();
            SceneManager.LoadScene(1);
        }
    }
}


