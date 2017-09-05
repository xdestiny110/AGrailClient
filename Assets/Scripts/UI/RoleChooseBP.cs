using UnityEngine;
using System.Collections;
using Framework.UI;
using System.Linq;
using UnityEngine.UI;
using System.Collections.Generic;
using Framework.AssetBundle;
using UnityEngine.SceneManagement;
using Framework.Message;

namespace AGrail
{
    public class RoleChooseBP : WindowsBase
    {
        [SerializeField]
        private List<PlayerIco> players;
        [SerializeField]
        private List<PlayerIco> pools;
        [SerializeField]
        private List<Text> nicknames;
        [SerializeField]
        private Text info;
        [SerializeField]
        private Image redHero;
        [SerializeField]
        private Image blueHero;
        [SerializeField]
        private Button redBP;
        [SerializeField]
        private Button blueBP;
        [SerializeField]
        private List<GameObject> seats;

        public override WindowType Type
        {
            get
            {
                return WindowType.RoleChooseBP;
            }
        }

        public override void Awake()
        {
            if (Lobby.Instance.SelectRoom.max_player != 6)
            {
                Destroy(players[0].gameObject);
                players.RemoveAt(0);
                Destroy(players[3].gameObject);
                players.RemoveAt(3);
                Destroy(nicknames[0].gameObject);
                nicknames.RemoveAt(0);
                Destroy(nicknames[3].gameObject);
                nicknames.RemoveAt(3);

                Destroy(pools[0].gameObject);
                pools.RemoveAt(0);
                Destroy(pools[0].gameObject);
                pools.RemoveAt(0);
                Destroy(pools[0].gameObject);
                pools.RemoveAt(0);
                Destroy(pools[0].gameObject);
                pools.RemoveAt(0);
                Destroy(seats[5].gameObject);
                Destroy(seats[4].gameObject);
                seats.RemoveAt(5);
                seats.RemoveAt(4);
            }
            var idx = BattleData.Instance.PlayerIdxOrder.IndexOf((int)BattleData.Instance.StartPlayerID);
            int a = 0, b = players.Count/2;
            for (int i = 0; i < players.Count; i++)
            {
                var player = BattleData.Instance.GetPlayerInfo((uint)BattleData.Instance.PlayerIdxOrder[(i + idx) % players.Count]);
                seats[i].transform.GetChild(0).GetComponent<Image>().sprite = (player.team == (uint)Team.Blue) ?
                    AssetBundleManager.Instance.LoadAsset<Sprite>("lobby_texture", "SeatBlue") :
                    AssetBundleManager.Instance.LoadAsset<Sprite>("lobby_texture", "SeatRed");
                if (player.team == (uint)Team.Blue)
                {
                    players[a].ID = player.id;
                    nicknames[a++].text = player.nickname;
                }
                else
                {
                    players[b].ID = player.id;
                    nicknames[b++].text = player.nickname;
                }
            }
            for (int i = 0; i < RoleChoose.Instance.RoleIDs.Count; i++)
            {
                pools[i].HeroID = RoleChoose.Instance.RoleIDs[i];
                pools[i].Selected = (RoleChoose.Instance.options[i] > 0);
                pools[i].canselect = RoleChoose.Instance.oprater == BattleData.Instance.PlayerID;
                uint id = RoleChoose.Instance.RoleIDs[i];
                pools[i].MVP.onClick.AddListener(delegate { onHeroClick(id); });
            }
            choosing = 0;
            redBP.onClick.AddListener(onSureClick);
            blueBP.onClick.AddListener(onSureClick);
            MessageSystem<MessageType>.Regist(MessageType.GameStart, this);
            MessageSystem<MessageType>.Regist(MessageType.PICKBAN, this);
            MessageSystem<MessageType>.Regist(MessageType.GAMEINFO, this);
            base.Awake();
        }

        public override void OnDestroy()
        {
            MessageSystem<MessageType>.UnRegist(MessageType.GameStart, this);
            MessageSystem<MessageType>.UnRegist(MessageType.PICKBAN, this);
            MessageSystem<MessageType>.UnRegist(MessageType.GAMEINFO, this);
            base.OnDestroy();
        }

        public override void OnEventTrigger(MessageType eventType, params object[] parameters)
        {
            switch (eventType)
            {
                case MessageType.GameStart:
                    if (coro != null)
                        StopCoroutine(coro);
                    SceneManager.LoadScene(2);
                    break;
                case MessageType.PICKBAN:
                    for (int i = 0; i < RoleChoose.Instance.RoleIDs.Count; i++)
                    {
                        pools[i].HeroID = RoleChoose.Instance.RoleIDs[i];
                        pools[i].Selected = (RoleChoose.Instance.options[i] > 0);
                        pools[i].canselect = (RoleChoose.Instance.oprater==BattleData.Instance.PlayerID) && !pools[i].Selected;
                    }
                    info.text = ((RoleChoose.Instance.oprater == BattleData.Instance.PlayerID) ? "请" :
                        ("等待<color=#ff0>" + BattleData.Instance.GetPlayerInfo(RoleChoose.Instance.oprater).nickname)+"</color>") +
                        ((RoleChoose.Instance.BPopration == network.BP_OPRATION.BP_BAN ? "<color=#ff0000>禁用</color>" : "选择要使用的")) + "角色";
                    break;
                case MessageType.GAMEINFO:
                    var BPInfo = parameters[0] as network.GameInfo;
                    foreach (var v in BPInfo.player_infos)
                    {
                        foreach (var c in players)
                        {
                            if (c.ID == v.id)
                            {
                                if (v.role_id != 0 && c.HeroID == 0)
                                {
                                    if (BattleData.Instance.GetPlayerInfo(c.ID).team == 0)
                                        blueHero.sprite = AssetBundleManager.Instance.LoadAsset<Sprite>("hero_l", v.role_id + "L");
                                    else
                                        redHero.sprite = AssetBundleManager.Instance.LoadAsset<Sprite>("hero_l", v.role_id + "L");
                                }
                                c.HeroID = v.role_id;
                            }
                        }
                    }
                    break;
            }
            base.OnEventTrigger(eventType, parameters);
        }

        private Coroutine coro = null;
        private uint choosing { get; set;}
        private void onHeroClick(uint id)
        {
            choosing = id;
            if (BattleData.Instance.MainPlayer.team == 0)
            {
                blueHero.sprite = AssetBundleManager.Instance.LoadAsset<Sprite>("hero_l", id + "L");
                blueBP.gameObject.SetActive(true);
                blueBP.GetComponent<Image>().sprite =
                    RoleChoose.Instance.BPopration == network.BP_OPRATION.BP_BAN ?
                    AssetBundleManager.Instance.LoadAsset<Sprite>("lobby_texture","r03-ban") :
                    AssetBundleManager.Instance.LoadAsset<Sprite>("lobby_texture","r03-pick");
            }
            else
            {
                redHero.sprite = AssetBundleManager.Instance.LoadAsset<Sprite>("hero_l", id + "L");
                redBP.gameObject.SetActive(true);
                redBP.GetComponent<Image>().sprite =
                    RoleChoose.Instance.BPopration == network.BP_OPRATION.BP_BAN ?
                    AssetBundleManager.Instance.LoadAsset<Sprite>("lobby_texture", "r03-ban") :
                    AssetBundleManager.Instance.LoadAsset<Sprite>("lobby_texture", "r03-pick");
            }
        }
        private void onSureClick()
        {
            foreach (var v in pools)
                v.canselect = false;
            blueBP.gameObject.SetActive(false);
            redBP.gameObject.SetActive(false);
            RoleChoose.Instance.Choose(choosing);

        }
    }
}


