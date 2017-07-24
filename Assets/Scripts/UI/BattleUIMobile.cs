using Framework.UI;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Framework.Message;
using Framework.AssetBundle;
using System.Linq;

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
        private List<PlayerStatusMobile> playerStatus;
        [SerializeField]
        private Transform showCardArea;
        [SerializeField]
        private Text hint;
        [SerializeField]
        private Text turn;

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
                playerStatus.RemoveAt(2);
                playerStatus.RemoveAt(3);                
            }
            
            MessageSystem<MessageType>.Regist(MessageType.MoraleChange, this);
            MessageSystem<MessageType>.Regist(MessageType.GemChange, this);
            MessageSystem<MessageType>.Regist(MessageType.CrystalChange, this);
            MessageSystem<MessageType>.Regist(MessageType.GrailChange, this);
            MessageSystem<MessageType>.Regist(MessageType.SendHint, this);
            MessageSystem<MessageType>.Regist(MessageType.PlayerNickName, this);
            MessageSystem<MessageType>.Regist(MessageType.PlayerTeamChange, this);
            MessageSystem<MessageType>.Regist(MessageType.PlayerRoleChange, this);
            MessageSystem<MessageType>.Regist(MessageType.PlayerHandChange, this);
            MessageSystem<MessageType>.Regist(MessageType.PlayerHealChange, this);
            MessageSystem<MessageType>.Regist(MessageType.PlayerTokenChange, this);
            MessageSystem<MessageType>.Regist(MessageType.PlayerKneltChange, this);
            MessageSystem<MessageType>.Regist(MessageType.PlayerEnergeChange, this);
            MessageSystem<MessageType>.Regist(MessageType.PlayerBasicAndExCardChange, this);
            MessageSystem<MessageType>.Regist(MessageType.CARDMSG, this);
            MessageSystem<MessageType>.Regist(MessageType.SKILLMSG, this);
            MessageSystem<MessageType>.Regist(MessageType.TURNBEGIN, this);

            //依据数据初始化界面
            MessageSystem<MessageType>.Notify(MessageType.MoraleChange, Team.Red, BattleData.Instance.Morale[(int)Team.Red]);
            MessageSystem<MessageType>.Notify(MessageType.MoraleChange, Team.Blue, BattleData.Instance.Morale[(int)Team.Blue]);            
            MessageSystem<MessageType>.Notify(MessageType.GemChange, Team.Red, (int)BattleData.Instance.Gem[(int)Team.Red]);
            MessageSystem<MessageType>.Notify(MessageType.GemChange, Team.Blue, (int)BattleData.Instance.Gem[(int)Team.Blue]);
            MessageSystem<MessageType>.Notify(MessageType.CrystalChange, Team.Red, (int)BattleData.Instance.Crystal[(int)Team.Red]);
            MessageSystem<MessageType>.Notify(MessageType.CrystalChange, Team.Blue, (int)BattleData.Instance.Crystal[(int)Team.Blue]);
            MessageSystem<MessageType>.Notify(MessageType.GrailChange, Team.Red, BattleData.Instance.Grail[(int)Team.Red]);
            MessageSystem<MessageType>.Notify(MessageType.GrailChange, Team.Blue, BattleData.Instance.Grail[(int)Team.Blue]);            

            var playerInfos = BattleData.Instance.PlayerInfos;
            var playerIdxOrder = BattleData.Instance.PlayerIdxOrder;
            if(playerIdxOrder.Count == playerInfos.Count && playerIdxOrder.Count > 0)
            {
                for (int i = 0; i < playerStatus.Count; i++)
                {
                    var playerInfo = playerInfos[playerIdxOrder[i]];
                    playerStatus[i].ID = playerInfo.id;
                    MessageSystem<MessageType>.Notify(MessageType.PlayerNickName, i, playerInfo.nickname);
                    MessageSystem<MessageType>.Notify(MessageType.PlayerTeamChange, i, playerInfo.team);
                    MessageSystem<MessageType>.Notify(MessageType.PlayerRoleChange, i, playerInfo.role_id);
                    MessageSystem<MessageType>.Notify(MessageType.PlayerHandChange, i, playerInfo.hands, playerInfo.max_hand);
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
            MessageSystem<MessageType>.UnRegist(MessageType.PlayerNickName, this);
            MessageSystem<MessageType>.UnRegist(MessageType.PlayerTeamChange, this);
            MessageSystem<MessageType>.UnRegist(MessageType.PlayerRoleChange, this);            
            MessageSystem<MessageType>.UnRegist(MessageType.PlayerHandChange, this);
            MessageSystem<MessageType>.UnRegist(MessageType.PlayerHealChange, this);
            MessageSystem<MessageType>.UnRegist(MessageType.PlayerTokenChange, this);
            MessageSystem<MessageType>.UnRegist(MessageType.PlayerKneltChange, this);
            MessageSystem<MessageType>.UnRegist(MessageType.PlayerEnergeChange, this);
            MessageSystem<MessageType>.UnRegist(MessageType.PlayerBasicAndExCardChange, this);
            MessageSystem<MessageType>.UnRegist(MessageType.CARDMSG, this);
            MessageSystem<MessageType>.UnRegist(MessageType.SKILLMSG, this);
            MessageSystem<MessageType>.UnRegist(MessageType.TURNBEGIN, this);
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
                    if (parameters.Length != 1)
                        hint.transform.parent.gameObject.SetActive(false);
                    else
                    {
                        hint.transform.parent.gameObject.SetActive(true);
                        hint.text = parameters[0].ToString();
                    }
                    break;
                case MessageType.PlayerNickName:
                    playerStatus[(int)parameters[0]].NickName = parameters[1].ToString();
                    break;
                case MessageType.PlayerTeamChange:
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
                            playerStatus[i].Turn = (BattleData.Instance.PlayerInfos[i].id == tb.id) ? true : false;
                    }
                    break;
            }
        }

        private void gemChange(Team team, int diffGem)
        {
            if (diffGem > 0)
            {
                var prefab = AssetBundleManager.Instance.LoadAsset("battle", "Gem");
                for (int i = 0; i < diffGem; i++)
                {
                    var go = Instantiate(prefab);
                    go.transform.SetParent(energies[(int)team]);
                    go.transform.localPosition = Vector3.zero;
                    go.transform.localRotation = Quaternion.identity;
                    go.transform.localScale = Vector3.one;
                    go.transform.SetSiblingIndex(0);
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
                var prefab = AssetBundleManager.Instance.LoadAsset("battle", "Crystal");
                for (int i = 0; i < diffCrystal; i++)
                {
                    var go = Instantiate(prefab);
                    go.transform.SetParent(energies[(int)team]);
                    go.transform.localPosition = Vector3.zero;
                    go.transform.localRotation = Quaternion.identity;
                    go.transform.localScale = Vector3.one;
                    go.transform.SetSiblingIndex(energies[(int)team].childCount - 1);
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
            var prefab = AssetBundleManager.Instance.LoadAsset("battle", "Grail");
            for (int i = 0; i < diffGrail; i++)
            {
                var go = Instantiate(prefab);
                go.transform.SetParent(grails[(int)team]);
                go.transform.localPosition = Vector3.zero;
                go.transform.localRotation = Quaternion.identity;
                go.transform.localScale = Vector3.one;
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
            //int srcIdx = -1, dstIdx = -1;
            //for (int i = 0; i < PlayersStatus.Count; i++)
            //{
            //    if (BattleData.Instance.PlayerInfos[i].id == src_id)
            //        srcIdx = i;
            //    if (BattleData.Instance.PlayerInfos[i].id == dst_id)
            //        dstIdx = i;
            //}

            //if (srcIdx < 0 || dstIdx < 0)
            //{
            //    Debug.LogErrorFormat("srcIdx = {0}, dstIdx = {1}", srcIdx, dstIdx);
            //    Debug.LogErrorFormat("srcID = {0}, dstID = {1}", src_id, dst_id);
            //    return;
            //}

            //var arrow = Instantiate(arrowPrefab);
            //arrow.transform.SetParent(battleRoot);
            //arrow.transform.position = PlayersStatus[srcIdx].transform.position;
            //arrow.transform.localScale = Vector3.one;
            //arrow.GetComponent<Arrow>().SetParms(PlayersStatus[srcIdx].transform.position, PlayersStatus[dstIdx].transform.position);
        }
    }
}


