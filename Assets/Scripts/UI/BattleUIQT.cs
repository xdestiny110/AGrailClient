using UnityEngine;
using Framework;
using Framework.UI;
using DG.Tweening;
using Framework.Message;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Linq;

namespace AGrail
{
    public class BattleUIQT : WindowsBase
    {
        [SerializeField]
        private Transform root;
        [SerializeField]
        private Transform battleRoot;
        [SerializeField]
        private Text turn;
        [SerializeField]
        private Text[] morales = new Text[2];
        [SerializeField]
        private Transform[] energy = new Transform[2];
        [SerializeField]
        private Transform[] grail = new Transform[2];
        [SerializeField]
        private List<Transform> playerAnchor = new List<Transform>();
        [SerializeField]
        private Transform ShowCardArea;
        [SerializeField]
        private Text dialog;
        [SerializeField]
        private GameObject playerStatusPrefab;
        [SerializeField]
        private Text txtHint;
        [SerializeField]
        private GameObject cardPrefab;
        [SerializeField]
        private GameObject arrowPrefab;

        public Dictionary<int, PlayerStatusQT> PlayersStatus = new Dictionary<int, PlayerStatusQT>();
        private int offset = 0;

        public override WindowType Type
        {
            get
            {
                throw new System.Exception();
                //return WindowType.BattleQT;
            }
        }

        private Tweener tween = null;

        public override void Awake()
        {
            //依据房间人数先去掉不存在anchor
            if(Lobby.Instance.SelectRoom.max_player != 6)
            {
                playerAnchor.RemoveAt(2);
                playerAnchor.RemoveAt(3);
            }
            //测试基本上都是先生成UI才会收到GameInfo事件
            //但不确定是否有可能反过来
            //最好是能够在Awake中先依据BattleData的数据初始化一遍
            Dialog.Instance.Reset();
            PlayersStatus.Clear();

            MessageSystem<MessageType>.Regist(MessageType.GameStart, this);
            MessageSystem<MessageType>.Regist(MessageType.ChooseRole, this);
            MessageSystem<MessageType>.Regist(MessageType.MoraleChange, this);
            MessageSystem<MessageType>.Regist(MessageType.GemChange, this);
            MessageSystem<MessageType>.Regist(MessageType.CrystalChange, this);
            MessageSystem<MessageType>.Regist(MessageType.GrailChange, this);
            MessageSystem<MessageType>.Regist(MessageType.SendHint, this);
            MessageSystem<MessageType>.Regist(MessageType.PlayerLeave, this);
            MessageSystem<MessageType>.Regist(MessageType.PlayerIsReady, this);
            MessageSystem<MessageType>.Regist(MessageType.PlayerNickName, this);
            MessageSystem<MessageType>.Regist(MessageType.PlayerRoleChange, this);
            MessageSystem<MessageType>.Regist(MessageType.PlayerTeamChange, this);
            MessageSystem<MessageType>.Regist(MessageType.PlayerHandChange, this);
            MessageSystem<MessageType>.Regist(MessageType.PlayerHealChange, this);
            MessageSystem<MessageType>.Regist(MessageType.PlayerTokenChange, this);
            MessageSystem<MessageType>.Regist(MessageType.PlayerKneltChange, this);
            MessageSystem<MessageType>.Regist(MessageType.PlayerEnergeChange, this);
            MessageSystem<MessageType>.Regist(MessageType.PlayerBasicAndExCardChange, this);
            MessageSystem<MessageType>.Regist(MessageType.LogChange, this);
            MessageSystem<MessageType>.Regist(MessageType.CARDMSG, this);
            MessageSystem<MessageType>.Regist(MessageType.SKILLMSG, this);
            MessageSystem<MessageType>.Regist(MessageType.TURNBEGIN, this);

            root.localPosition = new Vector3(Screen.width, 0, 0);
            root.DOLocalMoveX(0, 1.0f).OnComplete(() => { GameManager.AddUpdateAction(onESCClick); });
            base.Awake();
        }

        public override void OnDestroy()
        {
            PlayersStatus.Clear();
            MessageSystem<MessageType>.UnRegist(MessageType.GameStart, this);
            MessageSystem<MessageType>.UnRegist(MessageType.ChooseRole, this);
            MessageSystem<MessageType>.UnRegist(MessageType.MoraleChange, this);
            MessageSystem<MessageType>.UnRegist(MessageType.GemChange, this);
            MessageSystem<MessageType>.UnRegist(MessageType.CrystalChange, this);
            MessageSystem<MessageType>.UnRegist(MessageType.GrailChange, this);
            MessageSystem<MessageType>.UnRegist(MessageType.SendHint, this);
            MessageSystem<MessageType>.UnRegist(MessageType.PlayerLeave, this);
            MessageSystem<MessageType>.UnRegist(MessageType.PlayerIsReady, this);
            MessageSystem<MessageType>.UnRegist(MessageType.PlayerNickName, this);
            MessageSystem<MessageType>.UnRegist(MessageType.PlayerRoleChange, this);
            MessageSystem<MessageType>.UnRegist(MessageType.PlayerTeamChange, this);
            MessageSystem<MessageType>.UnRegist(MessageType.PlayerHandChange, this);
            MessageSystem<MessageType>.UnRegist(MessageType.PlayerHealChange, this);
            MessageSystem<MessageType>.UnRegist(MessageType.PlayerTokenChange, this);
            MessageSystem<MessageType>.UnRegist(MessageType.PlayerKneltChange, this);
            MessageSystem<MessageType>.UnRegist(MessageType.PlayerEnergeChange, this);
            MessageSystem<MessageType>.UnRegist(MessageType.PlayerBasicAndExCardChange, this);
            MessageSystem<MessageType>.UnRegist(MessageType.LogChange, this);
            MessageSystem<MessageType>.UnRegist(MessageType.CARDMSG, this);
            MessageSystem<MessageType>.UnRegist(MessageType.SKILLMSG, this);
            MessageSystem<MessageType>.UnRegist(MessageType.TURNBEGIN, this);

            GameManager.RemoveUpdateAciont(onESCClick);
            base.OnDestroy();
        }

        public override void OnEventTrigger(MessageType eventType, params object[] parameters)
        {
            switch (eventType)
            {
                case MessageType.GameStart:
                    adjustPlayerPosition(BattleData.Instance.PlayerIdxOrder);
                    break;
                case MessageType.ChooseRole:
                    var roleStrategy = (network.ROLE_STRATEGY)parameters[0];
                    switch (roleStrategy)
                    {
                        case network.ROLE_STRATEGY.ROLE_STRATEGY_31:
                            if (RoleChoose.Instance.RoleIDs.Count > 3)
                                GameManager.UIInstance.PushWindow(WindowType.RoleChooseAny, WinMsg.Pause);
                            else
                                GameManager.UIInstance.PushWindow(WindowType.RoleChoose31, WinMsg.Pause);
                            break;
                        default:
                            Debug.LogError("不支持的选将模式");
                            break;
                    }
                    break;
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
                    if (parameters.Length != 1)
                        txtHint.transform.parent.gameObject.SetActive(false);
                    else
                    {
                        txtHint.transform.parent.gameObject.SetActive(true);
                        txtHint.text = parameters[0].ToString();
                    }
                    break;
                case MessageType.PlayerNickName:
                    checkPlayer((int)parameters[0]);
                    PlayersStatus[(int)parameters[0]].PlayerName = (string)parameters[1];
                    break;
                case MessageType.PlayerRoleChange:
                    PlayersStatus[(int)parameters[0]].RoleID = (uint)parameters[1];
                    break;
                case MessageType.PlayerIsReady:
                    checkPlayer((int)parameters[0]);
                    PlayersStatus[(int)parameters[0]].IsReady = (bool)parameters[1];
                    break;
                case MessageType.PlayerLeave:
                    if (!BattleData.Instance.IsStarted)
                    {
                        //游戏未开始
                        Destroy(PlayersStatus[(int)parameters[1]].gameObject);
                        PlayersStatus.Remove((int)parameters[1]);
                    }
                    break;
                case MessageType.PlayerTeamChange:
                    checkPlayer((int)parameters[0]);
                    PlayersStatus[(int)parameters[0]].Team = (Team)(uint)parameters[1];
                    break;
                case MessageType.PlayerTokenChange:
                    var idx = (int)parameters[0];
                    PlayersStatus[(int)parameters[0]].YellowToken = (uint)parameters[1];
                    PlayersStatus[(int)parameters[0]].BlueToken = (uint)parameters[2];
                    PlayersStatus[(int)parameters[0]].Covered = (uint)parameters[3];
                    break;
                case MessageType.PlayerKneltChange:
                    PlayersStatus[(int)parameters[0]].Knelt = (bool)parameters[1];
                    break;
                case MessageType.PlayerBasicAndExCardChange:
                    PlayersStatus[(int)parameters[0]].BasicAndExCards = (parameters[1] as List<uint>).Union(parameters[2] as List<uint>).ToList();
                    break;
                case MessageType.PlayerEnergeChange:
                    PlayersStatus[(int)parameters[0]].Energy = new KeyValuePair<uint, uint>((uint)parameters[1], (uint)parameters[2]);
                    break;
                case MessageType.PlayerHandChange:
                    PlayersStatus[(int)parameters[0]].HandCount = new KeyValuePair<uint, uint>((uint)parameters[1], (uint)parameters[2]);
                    break;
                case MessageType.PlayerHealChange:
                    PlayersStatus[(int)parameters[0]].HealCount = (uint)parameters[1];
                    break;
                case MessageType.LogChange:
                    dialog.text = Dialog.Instance.Log;
                    break;
                case MessageType.CARDMSG:
                    var cardMsg = parameters[0] as network.CardMsg;
                    if((cardMsg.is_realSpecified && cardMsg.is_real && cardMsg.type == 1) || cardMsg.type == 2)
                        showCard(cardMsg.card_ids);
                    if (cardMsg.typeSpecified && cardMsg.type == 1 && cardMsg.dst_id != cardMsg.src_id)
                        actionAnim(cardMsg.src_id, cardMsg.dst_id);
                    break;
                case MessageType.SKILLMSG:
                    var skillMsg = parameters[0] as network.SkillMsg;
                    foreach (var v in skillMsg.dst_ids)
                        actionAnim(skillMsg.src_id, v);
                    break;
                case MessageType.TURNBEGIN:
                    var tb = parameters[0] as network.TurnBegin;
                    if (tb.roundSpecified)
                        turn.text = tb.round.ToString();
                    if (tb.idSpecified)
                    {
                        for (int i = 0; i < PlayersStatus.Count; i++)
                            PlayersStatus[i].IsTurn = (BattleData.Instance.PlayerInfos[i].id == tb.id) ? true : false;
                    }
                    break;
            }
        }

        private void onESCClick()
        {
            if (Input.GetKeyDown(KeyCode.Escape) && CanvasGroup.interactable)
            {
                //if (GameManager.UIInstance.PeekWindow() == WindowType.BattleQT)
                    //Lobby.Instance.LeaveRoom();
                GameManager.UIInstance.PopWindow(WinMsg.Show);
            }
        }

        private void gemChange(Team team, int diffGem)
        {
            if (diffGem > 0)
            {
                for (int i = 0; i < diffGem; i++)
                {
                    var go = new GameObject();
                    go.AddComponent<RawImage>().texture = Resources.Load<Texture2D>("Icons/gem");
                    go.transform.SetParent(energy[(int)team]);
                    go.transform.localPosition = Vector3.zero;
                    go.transform.localRotation = Quaternion.identity;
                    go.transform.localScale = Vector3.one;
                    go.transform.SetSiblingIndex(0);
                }
            }
            else
            {
                for (int i = 0; i < Mathf.Abs(diffGem); i++)
                    Destroy(energy[(int)team].GetChild(i).gameObject);
            }
        }

        private void crystalChange(Team team, int diffCrystal)
        {
            if (diffCrystal > 0)
            {
                for (int i = 0; i < diffCrystal; i++)
                {
                    var go = new GameObject();
                    go.AddComponent<RawImage>().texture = Resources.Load<Texture2D>("Icons/crystal");
                    go.transform.SetParent(energy[(int)team]);
                    go.transform.localPosition = Vector3.zero;
                    go.transform.localRotation = Quaternion.identity;
                    go.transform.localScale = Vector3.one;
                    go.transform.SetSiblingIndex(energy[(int)team].childCount - 1);
                }
            }
            else
            {
                for (int i = energy[(int)team].childCount - 1; i > energy[(int)team].childCount - 1 + diffCrystal; i--)
                    Destroy(energy[(int)team].GetChild(i).gameObject);
            }
        }

        private void grailChange(Team team, uint diffGrail)
        {
            for (int i = 0; i < diffGrail; i++)
            {
                var go = new GameObject();
                go.AddComponent<RawImage>().texture = Resources.Load<Texture2D>("Icons/grail");
                go.transform.SetParent(grail[(int)team]);
                go.transform.localPosition = Vector3.zero;
                go.transform.localRotation = Quaternion.identity;
                go.transform.localScale = Vector3.one;
            }
        }

        private void checkPlayer(int playerIdx)
        {
            if (!PlayersStatus.ContainsKey(playerIdx))
            {
                var go = Instantiate(playerStatusPrefab);
                if((BattleData.Instance.PlayerID == 9 && BattleData.Instance.PlayerInfos[playerIdx].id == 0) ||
                    BattleData.Instance.PlayerID == BattleData.Instance.PlayerInfos[playerIdx].id)
                {
                    //如果是主视角玩家
                    offset = playerIdx;
                    //调整现有的位置
                    for(int i = 0; i < PlayersStatus.Count; i++)
                    {
                        PlayersStatus[i].transform.SetParent(playerAnchor[i + playerAnchor.Count - offset]);
                        PlayersStatus[i].transform.localPosition = Vector3.zero;
                        PlayersStatus[i].transform.localRotation = Quaternion.identity;
                        PlayersStatus[i].transform.localScale = Vector3.one;
                    }
                }
                var id = BattleData.Instance.PlayerInfos[playerIdx].id;
                var status = go.GetComponent<PlayerStatusQT>();

                var anchor = playerAnchor[(playerIdx - offset < 0) ? playerIdx + playerAnchor.Count - offset : playerIdx - offset];
                go.transform.SetParent(anchor);
                go.transform.localPosition = Vector3.zero;
                go.transform.localRotation = Quaternion.identity;
                go.transform.localScale = Vector3.one;

                status.PlayerID = id;
                PlayersStatus.Add(playerIdx, status);
                status.AddBtnPlayerCallback(id);
            }
        }

        private void adjustPlayerPosition(List<int> playerIdxes)
        {
            for(int i = 0; i < playerIdxes.Count; i++)
            {
                PlayersStatus[playerIdxes[i]].transform.SetParent(playerAnchor[i]);
                PlayersStatus[playerIdxes[i]].transform.localPosition = Vector3.zero;
                PlayersStatus[playerIdxes[i]].transform.localRotation = Quaternion.identity;
                PlayersStatus[playerIdxes[i]].transform.localScale = Vector3.one;
            }
        }

        private void showCard(List<uint> card_ids)
        {
            for (int i = 0; i < ShowCardArea.childCount; i++)
                Destroy(ShowCardArea.GetChild(i).gameObject);
            foreach (var v in card_ids)
            {
                var card = Card.GetCard(v);
                var go = Instantiate(cardPrefab);
                go.transform.SetParent(ShowCardArea);
                go.transform.localScale = Vector3.one;
                go.transform.localPosition = Vector3.zero;
                go.transform.localRotation = Quaternion.identity;
                var cardUI = go.GetComponent<CardUI>();
                cardUI.Card = Card.GetCard(v);
                cardUI.Disappear();
            }
        }

        private void actionAnim(uint src_id, uint dst_id)
        {
            int srcIdx = -1, dstIdx = -1;
            for (int i = 0; i < PlayersStatus.Count; i++)
            {
                if (BattleData.Instance.PlayerInfos[i].id == src_id)
                    srcIdx = i;
                if (BattleData.Instance.PlayerInfos[i].id == dst_id)
                    dstIdx = i;
            }

            if (srcIdx < 0 || dstIdx < 0)
            {
                Debug.LogErrorFormat("srcIdx = {0}, dstIdx = {1}", srcIdx, dstIdx);
                Debug.LogErrorFormat("srcID = {0}, dstID = {1}", src_id, dst_id);
                return;
            }

            var arrow = Instantiate(arrowPrefab);
            arrow.transform.SetParent(battleRoot);
            arrow.transform.position = PlayersStatus[srcIdx].transform.position;
            arrow.transform.localScale = Vector3.one;
            arrow.GetComponent<Arrow>().SetParms(PlayersStatus[srcIdx].transform.position, PlayersStatus[dstIdx].transform.position);
        }
    }
}
