using UnityEngine;
using System.Collections;
using Framework.UI;
using System.Linq;
using UnityEngine.UI;
using System.Collections.Generic;
using Framework.AssetBundle;
using UnityEngine.SceneManagement;
using Framework.Message;
using DG.Tweening;

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
        private Button BPBtn;
        [SerializeField]
        private Button CancelBtn;
        [SerializeField]
        private List<Image> seats;
        [SerializeField]
        private Image pointer;
        [SerializeField]
        private List<Text> Hname;
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
        private Transform FlagGroup;
        [SerializeField]
        private Transform RedGroup;
        [SerializeField]
        private Transform BlueGroup;
        [SerializeField]
        private Image BlueLeader;
        [SerializeField]
        private Image RedLeader;

        public override WindowType Type
        {
            get
            {
                return WindowType.RoleChooseBPCM;
            }
        }

        private void Exchange<T>(T a,T b)
        {
            T t = a;
            a = b;
            b = t;
        }

        private void ExchangeRt(RectTransform a,RectTransform b)
        {
            Exchange(a.anchorMin, b.anchorMin);
            Exchange(a.anchorMax, b.anchorMax);
            Exchange(a.anchoredPosition, b.anchoredPosition);
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
            if (BattleData.Instance.MainPlayer.team == 0)
            {             
                ExchangeRt(RedGroup.GetComponent<RectTransform>(), BlueGroup.GetComponent<RectTransform>());
                ExchangeRt(redHero.GetComponent<RectTransform>(), blueHero.GetComponent<RectTransform>());
                
                //RedGroup.localPosition = new Vector3(450, 300, 0);
                //BlueGroup.localPosition = new Vector3(-450, 300, 0);
                //redHero.transform.localPosition = new Vector3(370, -100, 0);
                //blueHero.transform.localPosition = new Vector3(-370, -100, 0);
                FlagGroup.localScale = new Vector3(-1, 1, 0);
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
                    if (player.leader == 1)
                    {
                        BlueLeader.transform.SetParent(players[a].transform);
                        BlueLeader.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, -75);
                        //BlueLeader.transform.localPosition = new Vector3(0, 35, 0);
                        BlueLeader.gameObject.SetActive(true);
                    }
                    nicknames[a++].text = player.nickname;

                }
                else
                {
                    players[b].ID = player.id;
                    players[b].Seat = i;
                    if (player.leader == 1)
                    {
                        RedLeader.transform.SetParent(players[b].transform);
                        RedLeader.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, -75);
                        //RedLeader.transform.localPosition = new Vector3(0, 35, 0);
                        RedLeader.gameObject.SetActive(true);
                    }
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
            lastChosen = 0;
            BPBtn.onClick.AddListener(onSureClick);
            CancelBtn.onClick.AddListener(cancelIB);
            btnLogExpand.onClick.AddListener(delegate { onBtnLogExpandClick(true); });
            btnChatExpand.onClick.AddListener(delegate { onBtnChatExpandClick(true); });
            btnSubmit.onClick.AddListener(delegate { onBtnSubmitClick(null); });
            inptChat.onEndEdit.AddListener(onBtnSubmitClick);

            MessageSystem<MessageType>.Regist(MessageType.ChatChange, this);
            MessageSystem<MessageType>.Regist(MessageType.GameStart, this);
            MessageSystem<MessageType>.Regist(MessageType.PICKBAN, this);
            MessageSystem<MessageType>.Regist(MessageType.GAMEINFO, this);

            base.Awake();
        }

        private void Start()
        {
            logRoot.gameObject.SetActive(false);
            chatRoot.gameObject.SetActive(false);
        }

        public override void OnDestroy()
        {
            MessageSystem<MessageType>.Regist(MessageType.ChatChange, this);
            MessageSystem<MessageType>.UnRegist(MessageType.GameStart, this);
            MessageSystem<MessageType>.UnRegist(MessageType.PICKBAN, this);
            MessageSystem<MessageType>.UnRegist(MessageType.GAMEINFO, this);
            base.OnDestroy();
        }

        private int lastChosen { get; set; }
        private bool start = false;

        public override void OnEventTrigger(MessageType eventType, params object[] parameters)
        {
            switch (eventType)
            {
                case MessageType.PICKBAN:
                    bool changed = false;
                    for (int i = 0; i < RoleChoose.Instance.RoleIDs.Count; i++)
                    {
                        pools[i].HeroID = RoleChoose.Instance.RoleIDs[i];
                        if(RoleChoose.Instance.options[i] > 0)
                        {
                            start = true;
                            if (!pools[i].Selected)
                            { 
                            pools[i].Selected = true;
                            changed = true;
                            lastChosen = i;
                            }
                        }
                        pools[i].canselect = (RoleChoose.Instance.oprater==BattleData.Instance.PlayerID) && !pools[i].Selected;
                    }
                    if (RoleChoose.Instance.oprater == BattleData.Instance.PlayerID && RoleChoose.Instance.opration == 4)
                            CancelBtn.gameObject.SetActive(true);
                    if (RoleChoose.Instance.opration != 1)
                    {
                        string who = (RoleChoose.Instance.oprater == BattleData.Instance.PlayerID) ? "轮到你" :
                            (("等待<color=#ff0>" + BattleData.Instance.GetPlayerInfo(RoleChoose.Instance.oprater).nickname) + "</color>");
                        string what = "";

                        switch (RoleChoose.Instance.opration)
                        {
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
                        info.text = who + what + "角色";
                    }
                    if (RoleChoose.Instance.opration==1 && changed)
                    {
                        if(RoleChoose.Instance.RoleStrategy == network.ROLE_STRATEGY.ROLE_STRATEGY_CM)
                        { 
                        string what2 = "";
                        switch (RoleChoose.Instance.options[lastChosen])
                        { 
                            case 12:
                            case 15:
                                what2 = "　禁用了　";
                                break;
                            case 14:
                            case 17:
                                what2 = "　选择了　";
                                break;
                            case 13:
                            case 16:
                                what2 = "　额外禁用了　";
                                break;
                        }
                        log.text += "<color=#ff0>" + BattleData.Instance.GetPlayerInfo(RoleChoose.Instance.oprater).nickname + "</color>" + what2 + "<color=#ff0>"+ RoleFactory.Create(RoleChoose.Instance.RoleIDs[lastChosen]).RoleName+ "</color>"+"\n";
                        }
                        if (RoleChoose.Instance.RoleStrategy == network.ROLE_STRATEGY.ROLE_STRATEGY_BP)
                        { 
                            int isPick = ((RoleChoose.Instance.options[lastChosen] - 1) / 2) % 2;
                            log.text += "<color=#ff0>" + BattleData.Instance.GetPlayerInfo(RoleChoose.Instance.oprater).nickname + "</color>" + (isPick == 0 ? "　禁用了　" : "　选择了　") + "<color=#ff0>" + RoleFactory.Create(RoleChoose.Instance.RoleIDs[lastChosen]).RoleName + "</color>" + "\n";
                        }
                    }
                    if(!start)
                    {
                        start = true;
                        log.text += "以下角色响应了召集　";
                        foreach (var v in RoleChoose.Instance.RoleIDs)
                            log.text += "<color=#ff0>"+RoleFactory.Create(v).RoleName+"</color>　";
                        log.text += "\n";
                    }
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
                    int a = count / 2;
                    if (count % 2 == 0)
                        pointer.transform.localPosition = new Vector3(-250 + ((players[a - 1].Seat) * 100), pointer.transform.localPosition.y, pointer.transform.localPosition.z);
                    else
                        pointer.transform.localPosition = new Vector3(-250+ ( (players[a + players.Count/2].Seat) * 100), pointer.transform.localPosition.y, pointer.transform.localPosition.z);
                    break;
                case MessageType.ChatChange:
                    chatChange();
                    break;
            }
            base.OnEventTrigger(eventType, parameters);
        }
        
        private uint choosing { get; set;}
        private void onHeroClick(uint id)
        {
            choosing = id;
            BPBtn.gameObject.SetActive(true);
            BPBtn.GetComponent<Image>().sprite =
                RoleChoose.Instance.opration == 3 ?
                AssetBundleManager.Instance.LoadAsset<Sprite>("lobby_texture", "r03-pick") :
                AssetBundleManager.Instance.LoadAsset<Sprite>("lobby_texture", "r03-ban");
            if (BattleData.Instance.MainPlayer.team == 0)
                blueHero.sprite = AssetBundleManager.Instance.LoadAsset<Sprite>("hero_l", id + "L");
            else
                redHero.sprite = AssetBundleManager.Instance.LoadAsset<Sprite>("hero_l", id + "L");
        }
        private void onSureClick()
        {
            foreach (var v in pools)
                v.canselect = false;
            BPBtn.gameObject.SetActive(false);
            CancelBtn.gameObject.SetActive(false);
            RoleChoose.Instance.Choose(choosing);

        }

        private void cancelIB()
        {
            foreach (var v in pools)
                v.canselect = false;
            BPBtn.gameObject.SetActive(false);
            CancelBtn.gameObject.SetActive(false);
            RoleChoose.Instance.Choose(100);

        }

        private void chatChange()
        {
            var go = Instantiate<GameObject>(AssetBundleManager.Instance.LoadAsset("battle", "ChatItem"));
            go.transform.SetParent(this.chat);
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
            //playerStatus[BattleData.Instance.PlayerIdxOrder.IndexOf((int)chat.ID)].Chat = chat.msg;
        }

        private Tweener logTweener = null;
        private void onBtnLogExpandClick(bool flag)
        {
            if (logTweener != null && logTweener.IsPlaying())
                return;
            btnLogExpand.onClick.RemoveAllListeners();
            var rt = logRoot.GetComponent<RectTransform>();
            var w = rt.sizeDelta.x;
            if (flag)
            {
                logRoot.gameObject.SetActive(true);
                rt.anchoredPosition = new Vector2(-w * .5f, 0);
                //logTweener = logRoot.DOLocalMoveX(320, 0.5f);
                btnLogExpand.onClick.AddListener(delegate { onBtnLogExpandClick(false); });
            }
            else
            {
                rt.anchoredPosition = new Vector2(w * .5f, 0);
                logRoot.gameObject.SetActive(false);
                btnLogExpand.onClick.AddListener(delegate { onBtnLogExpandClick(true); });
            }
        }

        private Tweener chatTweener = null;
        private void onBtnChatExpandClick(bool flag)
        {
            if (chatTweener != null && chatTweener.IsPlaying())
                return;
            btnChatExpand.onClick.RemoveAllListeners();
            var rt = chatRoot.GetComponent<RectTransform>();
            var width = rt.sizeDelta.x;
            if (flag)
            {
                chatRoot.gameObject.SetActive(true);
                rt.anchoredPosition = new Vector2(width * .5f, 0);
                btnChatExpand.onClick.AddListener(delegate { onBtnChatExpandClick(false); });
            }
            else
            {
                rt.anchoredPosition = new Vector2(-width * .5f, 0);
                chatRoot.gameObject.SetActive(false);
                btnChatExpand.onClick.AddListener(delegate { onBtnChatExpandClick(true); });
            }
        }

        private void onBtnSubmitClick(string str)
        {
            if (!string.IsNullOrEmpty(inptChat.text))
                Dialog.Instance.SendTalk(inptChat.text);
            inptChat.text = string.Empty;
        }
    }
}


