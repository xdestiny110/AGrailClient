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
    public class RoleChooseBPCM : WindowsBase
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
        private Button redCancel;
        [SerializeField]
        private Button blueCancel;
        [SerializeField]
        private List<Image> seats;
        [SerializeField]
        private Image pointer;
        [SerializeField]
        private List<Text> Hname;
        public override WindowType Type
        {
            get
            {
                return WindowType.RoleChooseBPCM;
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
                seats[i].sprite = (player.team == (uint)Team.Blue) ?
                    AssetBundleManager.Instance.LoadAsset<Sprite>("lobby_texture", "seatBlueSquare") :
                    AssetBundleManager.Instance.LoadAsset<Sprite>("lobby_texture", "seatRedSquare");
                if (player.team == (uint)Team.Blue)
                {
                    players[a].ID = player.id;
                    players[a].Seat = i;
                    nicknames[a++].text = player.nickname;
                }
                else
                {
                    players[b].ID = player.id;
                    players[b].Seat = i;
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
            redCancel.onClick.AddListener(cancelIB);
            blueCancel.onClick.AddListener(cancelIB);
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
                    SceneManager.LoadScene(2);
                    break;
                case MessageType.PICKBAN:
                    for (int i = 0; i < RoleChoose.Instance.RoleIDs.Count; i++)
                    {
                        pools[i].HeroID = RoleChoose.Instance.RoleIDs[i];
                        pools[i].Selected = (RoleChoose.Instance.options[i] > 0);
                        pools[i].canselect = (RoleChoose.Instance.oprater==BattleData.Instance.PlayerID) && !pools[i].Selected;
                    }
                    if (RoleChoose.Instance.oprater == BattleData.Instance.PlayerID && RoleChoose.Instance.opration == 4)
                        if (BattleData.Instance.MainPlayer.team == 0)
                            blueCancel.gameObject.SetActive(RoleChoose.Instance.opration == 4);
                        else
                            redCancel.gameObject.SetActive(RoleChoose.Instance.opration == 4);

                    string who = (RoleChoose.Instance.oprater == BattleData.Instance.PlayerID) ? "轮到你" :
                        (("等待<color=#ff0>" + BattleData.Instance.GetPlayerInfo(RoleChoose.Instance.oprater).nickname) + "</color>");
                    string what="";
                    switch (RoleChoose.Instance.opration)
                    {
                        case 1:
                            what = "错误";
                            break;
                        case 2:
                            what = "<color=#ff0000>禁用</color>";
                            break;
                        case 3:
                            what = "选择";
                            break;
                        case 4:
                            what = "选择是否额外<color=#ff0000>禁用</color>";
                            break;
                    }
                    info.text = who + what + "角色" ;
                    break;
                case MessageType.GAMEINFO:
                    var BPInfo = parameters[0] as network.GameInfo;
                    int count = 1;
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
                                Hname[c.Seat].text = RoleFactory.Create(v.role_id).ShortName;
                            }

                        }
                        if (v.role_id != 0)
                            count++;
                    }
                    Debug.LogError(count);
                    int a = count / 2;
                    if (count % 2 == 0)
                        pointer.transform.localPosition = new Vector3(-250 + ((players[a - 1].Seat) * 100), pointer.transform.localPosition.y, pointer.transform.localPosition.z);
                    else
                        pointer.transform.localPosition = new Vector3(-250+ ( (players[a + players.Count/2].Seat) * 100), pointer.transform.localPosition.y, pointer.transform.localPosition.z);
                    break;
            }
            base.OnEventTrigger(eventType, parameters);
        }
        
        private uint choosing { get; set;}
        private void onHeroClick(uint id)
        {
            choosing = id;
            if (BattleData.Instance.MainPlayer.team == 0)
            {
                blueHero.sprite = AssetBundleManager.Instance.LoadAsset<Sprite>("hero_l", id + "L");
                blueBP.gameObject.SetActive(true);
                blueBP.GetComponent<Image>().sprite =
                    RoleChoose.Instance.opration == 3 ?
                    AssetBundleManager.Instance.LoadAsset<Sprite>("lobby_texture","r03-pick") :
                    AssetBundleManager.Instance.LoadAsset<Sprite>("lobby_texture","r03-ban");
            }
            else
            {
                redHero.sprite = AssetBundleManager.Instance.LoadAsset<Sprite>("hero_l", id + "L");
                redBP.gameObject.SetActive(true);
                redBP.GetComponent<Image>().sprite =
                    RoleChoose.Instance.opration == 3 ?
                    AssetBundleManager.Instance.LoadAsset<Sprite>("lobby_texture", "r03-pick") :
                    AssetBundleManager.Instance.LoadAsset<Sprite>("lobby_texture", "r03-ban");
            }
        }
        private void onSureClick()
        {
            foreach (var v in pools)
                v.canselect = false;
            blueBP.gameObject.SetActive(false);
            blueCancel.gameObject.SetActive(false);
            redBP.gameObject.SetActive(false);
            redCancel.gameObject.SetActive(false);
            RoleChoose.Instance.Choose(choosing);

        }

        private void cancelIB()
        {
            foreach (var v in pools)
                v.canselect = false;
            blueBP.gameObject.SetActive(false);
            blueCancel.gameObject.SetActive(false);
            redBP.gameObject.SetActive(false);
            redCancel.gameObject.SetActive(false);
            RoleChoose.Instance.Choose(100);

        }
    }
}


