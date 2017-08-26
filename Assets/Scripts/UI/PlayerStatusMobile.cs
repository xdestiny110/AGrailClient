using Framework.Message;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Framework.AssetBundle;
using Framework.UI;
using DG.Tweening;
using System.Collections;

namespace AGrail
{
    [RequireComponent(typeof(LongPress))]
    [RequireComponent(typeof(Button))]
    public class PlayerStatusMobile : MonoBehaviour, IMessageListener<MessageType>
    {
        [SerializeField]
        private Image redTeam;
        [SerializeField]
        private Image blueTeam;
        [SerializeField]
        private Image turn;
        [SerializeField]
        private Image select;
        [SerializeField]
        private Image canSelect;
        [SerializeField]
        private Image hero;
        [SerializeField]
        private Image property;
        [SerializeField]
        private Image knelt;
        [SerializeField]
        private Text playerName;
        [SerializeField]
        private Text chat;
        [SerializeField]
        private Text waitAction;
        [SerializeField]
        private Transform handArea;
        [SerializeField]
        private Transform healArea;
        [SerializeField]
        private Transform energyArea;
        [SerializeField]
        private Transform basicAndExCardArea;

        public Transform AnimationPos;
        private RoleBase role = RoleFactory.Create(AGrail.RoleID.Blank);
        private Text token0, token1, token2;
        private Button btnPlayer;
        private Animation hurted;

        public uint ID { get; set; }

        public bool IsEnable
        {
            set
            {
                btnPlayer.interactable = value;
                canSelect.gameObject.SetActive(value);
                foreach(var v in GetComponentsInChildren<Image>())
                {
                    var c = v.color;
                    c.b = c.r = c.g = 1;
                    if (BattleData.Instance.PlayerID != 9 &&
                        (BattleData.Instance.Agent.FSM.Current.StateNumber == (uint)StateEnum.Any ||
                        BattleData.Instance.Agent.FSM.Current.StateNumber == (uint)StateEnum.AttackAndMagic ||
                        BattleData.Instance.Agent.FSM.Current.StateNumber == (uint)StateEnum.Attack ||
                        BattleData.Instance.Agent.FSM.Current.StateNumber == (uint)StateEnum.Attacked ||
                        BattleData.Instance.Agent.FSM.Current.StateNumber == (uint)StateEnum.Magic ||
                        BattleData.Instance.Agent.FSM.Current.StateNumber == (uint)StateEnum.Modaned ||
                        BattleData.Instance.Agent.FSM.Current.StateNumber == (uint)StateEnum.Extract ||
                        BattleData.Instance.Agent.FSM.Current.StateNumber >= 100) && !value)
                        c.b = c.r = c.g = 0.5f;
                    v.color = c;
                }
            }
        }

        public string NickName
        {
            set
            {
                playerName.text = value;
            }
        }

        public uint RoleID
        {
            set
            {
                role = RoleFactory.Create(value);
                Sprite sprite;
                if (ID == BattleData.Instance.PlayerID || (ID == BattleData.Instance.PlayerInfos[0].id && BattleData.Instance.PlayerID == 9))
                    sprite = AssetBundleManager.Instance.LoadAsset<Sprite>("hero_l", value.ToString() + "L");
                else
                    sprite = AssetBundleManager.Instance.LoadAsset<Sprite>("hero_s", value.ToString() + "S");
                if (sprite != null)
                    hero.sprite = sprite;
                property.sprite = AssetBundleManager.Instance.LoadAsset<Sprite>("battle_texture", role.RoleProperty.ToString());
                if (role.HasYellow)
                {
                    var prefab = AssetBundleManager.Instance.LoadAsset("battle", "Token0");
                    var go = Instantiate(prefab);
                    go.name = "token0";
                    go.transform.SetParent(basicAndExCardArea);
                    go.transform.localPosition = prefab.transform.localPosition;
                    go.transform.localRotation = prefab.transform.localRotation;
                    go.transform.localScale = prefab.transform.localScale;
                    token0 = go.transform.GetChild(0).GetComponent<Text>();
                    token0.text = "0";
                }
                if (role.HasBlue)
                {
                    var prefab = AssetBundleManager.Instance.LoadAsset("battle", "Token1");
                    var go = Instantiate(prefab);
                    go.name = "token1";
                    go.transform.SetParent(basicAndExCardArea);
                    go.transform.localPosition = prefab.transform.localPosition;
                    go.transform.localRotation = prefab.transform.localRotation;
                    go.transform.localScale = prefab.transform.localScale;
                    token1 = go.transform.GetChild(0).GetComponent<Text>();
                    token1.text = "0";
                }
                if (role.HasCoverd)
                {
                    var prefab = AssetBundleManager.Instance.LoadAsset("battle", "Token2");
                    var go = Instantiate(prefab);
                    go.name = "token2";
                    go.transform.SetParent(basicAndExCardArea);
                    go.transform.localPosition = prefab.transform.localPosition;
                    go.transform.localRotation = prefab.transform.localRotation;
                    go.transform.localScale = prefab.transform.localScale;
                    token2 = go.transform.GetChild(0).GetComponent<Text>();
                    token2.text = "0";
                }
            }
        }

        public Team Team
        {
            set
            {
                if(value == Team.Blue)
                {
                    redTeam.enabled = false;
                    blueTeam.enabled = true;
                }
                else
                {
                    redTeam.enabled = true;
                    blueTeam.enabled = false;
                }
            }
        }

        public bool Turn
        {
            set
            {
                turn.enabled = value;
            }
        }

        public bool Knelt
        {
            set
            {
                if (value)
                {
                    knelt.sprite = AssetBundleManager.Instance.LoadAsset<Sprite>("battle_texture", role.Knelt);
                    knelt.gameObject.SetActive(true);
                }
                else
                    knelt.gameObject.SetActive(false);
            }
        }

        public uint YellowToken
        {
            set
            {
                if (token0 != null)
                    token0.text = value.ToString();
            }
        }

        public uint BlueToken
        {
            set
            {
                if (token1 != null)
                    token1.text = value.ToString();
            }
        }

        public uint Covered
        {
            set
            {
                if (token2 != null)
                    token2.text = value.ToString();
            }
        }

        public List<uint> BasicAndExCards
        {
            set
            {
                for (int i = 0; i < basicAndExCardArea.childCount; i++)
                {
                    if (basicAndExCardArea.GetChild(i).name.StartsWith("token")) continue;
                    Destroy(basicAndExCardArea.GetChild(i).gameObject);
                }

                foreach (var v in value)
                {
                    var card = Card.GetCard(v);
                    GameObject prefab = AssetBundleManager.Instance.LoadAsset("battle", "Image");
                    var go = Instantiate(prefab);

                    if (card.Name == Card.CardName.中毒)
                        go.GetComponent<Image>().sprite = AssetBundleManager.Instance.LoadAsset<Sprite>("battle_texture", "du");
                    else if (card.Name == Card.CardName.圣盾 || card.HasSkill("天使之墙"))
                        go.GetComponent<Image>().sprite = AssetBundleManager.Instance.LoadAsset<Sprite>("battle_texture", "dun");
                    else if (card.Name == Card.CardName.虚弱)
                        go.GetComponent<Image>().sprite = AssetBundleManager.Instance.LoadAsset<Sprite>("battle_texture", "xu");
                    else if (card.HasSkill("威力赐福"))
                        go.GetComponent<Image>().sprite = AssetBundleManager.Instance.LoadAsset<Sprite>("battle_texture", "wei");
                    else if (card.HasSkill("迅捷赐福"))
                        go.GetComponent<Image>().sprite = AssetBundleManager.Instance.LoadAsset<Sprite>("battle_texture", "xun");
                    else if (card.HasSkill("地之封印"))
                        go.GetComponent<Image>().sprite = AssetBundleManager.Instance.LoadAsset<Sprite>("battle_texture", "difeng");
                    else if (card.HasSkill("火之封印"))
                        go.GetComponent<Image>().sprite = AssetBundleManager.Instance.LoadAsset<Sprite>("battle_texture", "huofeng");
                    else if (card.HasSkill("水之封印"))
                        go.GetComponent<Image>().sprite = AssetBundleManager.Instance.LoadAsset<Sprite>("battle_texture", "shuifeng");
                    else if (card.HasSkill("风之封印"))
                        go.GetComponent<Image>().sprite = AssetBundleManager.Instance.LoadAsset<Sprite>("battle_texture", "fengfeng");
                    else if (card.HasSkill("雷之封印"))
                        go.GetComponent<Image>().sprite = AssetBundleManager.Instance.LoadAsset<Sprite>("battle_texture", "leifeng");
                    else if (card.Name == Card.CardName.五行束缚)
                        go.GetComponent<Image>().sprite = AssetBundleManager.Instance.LoadAsset<Sprite>("battle_texture", "shufu");
                    else if (card.Name == Card.CardName.挑衅)
                        go.GetComponent<Image>().sprite = AssetBundleManager.Instance.LoadAsset<Sprite>("battle_texture", "tiaoxin");
                    else if (card.Name == Card.CardName.灵魂链接)
                        go.GetComponent<Image>().sprite = AssetBundleManager.Instance.LoadAsset<Sprite>("battle_texture", "lianjie");
                    else if (card.Name == Card.CardName.同生共死)
                        go.GetComponent<Image>().sprite = AssetBundleManager.Instance.LoadAsset<Sprite>("battle_texture", "tongsheng");
                    else if (card.Name == Card.CardName.永恒乐章)
                        go.GetComponent<Image>().sprite = AssetBundleManager.Instance.LoadAsset<Sprite>("battle_texture", "yuezhang");
                    else {
                        Debug.LogErrorFormat("Basic card is error! CardID = {0}", v);
                        return;
                    }
                    addChildGO(basicAndExCardArea, go);
                }
            }
        }

        public KeyValuePair<uint, uint> Energy
        {
            set
            {
                for (int i = 0; i < energyArea.childCount; i++)
                    Destroy(energyArea.GetChild(i).gameObject);
                var prefab = AssetBundleManager.Instance.LoadAsset("battle", "Image");
                for(int i = 0; i < (int)value.Key; i++)
                {
                    var go = Instantiate(prefab);
                    go.GetComponent<Image>().sprite = AssetBundleManager.Instance.LoadAsset<Sprite>("battle_texture", "gem");
                    addChildGO(energyArea, go);
                }

                for (int i = 0; i < (int)value.Value; i++)
                {
                    var go = Instantiate(prefab);
                    go.GetComponent<Image>().sprite = AssetBundleManager.Instance.LoadAsset<Sprite>("battle_texture", "crystal");
                    addChildGO(energyArea, go);
                }
            }
        }

        public KeyValuePair<uint, uint> HandCount
        {
            set
            {
                for (int i = 0; i < handArea.childCount; i++)
                    Destroy(handArea.GetChild(i).gameObject);
                var prefab = AssetBundleManager.Instance.LoadAsset("battle", "Image");
                for (int i = 0; i < value.Key; i++)
                {
                    var go = Instantiate(prefab);
                    if (i >= value.Value)
                        go.GetComponent<Image>().sprite = AssetBundleManager.Instance.LoadAsset<Sprite>("battle_texture", "handcardfull");
                    else
                        go.GetComponent<Image>().sprite = AssetBundleManager.Instance.LoadAsset<Sprite>("battle_texture", "handcard");
                    addChildGO(handArea, go);
                }
                for (int i = 0; i < (int)value.Value - (int)value.Key; i++)
                {
                    var go = Instantiate(prefab);
                    go.GetComponent<Image>().sprite = AssetBundleManager.Instance.LoadAsset<Sprite>("battle_texture", "handcardempty");
                    addChildGO(handArea, go);
                }
            }
        }

        public uint HealCount
        {
            set
            {
                for (int i = 0; i < healArea.childCount; i++)
                    Destroy(healArea.GetChild(i).gameObject);
                var prefab = AssetBundleManager.Instance.LoadAsset("battle", "Image");
                for (int i = 0; i < (int)value; i++)
                {
                    var go = Instantiate(prefab);
                    go.GetComponent<Image>().sprite = AssetBundleManager.Instance.LoadAsset<Sprite>("battle_texture", "heal");
                    addChildGO(healArea, go);
                }
                for (int i = 0; i < (int)role.MaxHealCount - (int)value; i++)
                {
                    var go = Instantiate(prefab);
                    go.GetComponent<Image>().sprite = AssetBundleManager.Instance.LoadAsset<Sprite>("battle_texture", "handcardempty");
                    addChildGO(healArea, go);
                }
            }
        }

        private Coroutine chatCoro = null;
        public string Chat
        {
            set
            {
                if (chatCoro != null)
                    StopCoroutine(chatCoro);
                chat.transform.parent.gameObject.SetActive(true);
                chat.text = value;
                chatCoro = StartCoroutine(chatDisappear());
            }
        }

        public string WaitAction
        {
            set
            {
                waitAction.text = value;
                if (string.IsNullOrEmpty(waitAction.text))
                    waitAction.transform.parent.gameObject.SetActive(false);
                else
                    waitAction.transform.parent.gameObject.SetActive(true);
            }
        }

        void Awake()
        {
            btnPlayer = GetComponent<Button>();
            btnPlayer.onClick.AddListener(onClick);
            GetComponent<LongPress>().OnLongPress.AddListener(onLongPress);
            hurted = GetComponent<Animation>();
            MessageSystem<MessageType>.Regist(MessageType.AgentSelectPlayer, this);
        }

        void OnDestroy()
        {
            MessageSystem<MessageType>.UnRegist(MessageType.AgentSelectPlayer, this);
        }

        public void OnEventTrigger(MessageType eventType, params object[] parameters)
        {
            switch (eventType)
            {
                case MessageType.AgentSelectPlayer:
                    if (BattleData.Instance.Agent.SelectPlayers.Contains(ID))
                    {
                        select.gameObject.SetActive(true);
                        canSelect.gameObject.SetActive(false);
                    }
                    else
                    {
                        select.gameObject.SetActive(false);
                        canSelect.gameObject.SetActive(btnPlayer.interactable);
                    }
                    break;
            }
        }

        public void TurnBegin()
        {
            Turn = true;
            if(ID == BattleData.Instance.PlayerID || (BattleData.Instance.PlayerID == 9 && ID == BattleData.Instance.PlayerIdxOrder[0]))
            {
                turn.transform.localScale = new Vector3(2.5f, 2.5f, 1);
                turn.transform.DOScale(new Vector3(2, 2, 1), 1.5f);
            }
            else
            {
                turn.transform.localScale = new Vector3(2, 2, 1);
                turn.transform.DOScale(new Vector3(1.2f, 1.2f, 1), 1.5f);
            }
        }

        public void Hurted()
        {
            Debug.Log("hurted animation");
            if (!hurted.isPlaying)
                hurted.Play();
        }

        private void onClick()
        {
            if (!select.isActiveAndEnabled)
                BattleData.Instance.Agent.AddSelectPlayer(ID);
            else
                BattleData.Instance.Agent.RemoveSelectPlayer(ID);
        }

        private void onLongPress()
        {
            if(role != null)
            GameManager.UIInstance.PushWindow(Framework.UI.WindowType.InfomationUI, Framework.UI.WinMsg.None, -1, Vector3.zero, role);
        }

        private void addChildGO(Transform parent, GameObject go)
        {
            go.transform.SetParent(parent);
            go.transform.localPosition = Vector3.zero;
            go.transform.localRotation = Quaternion.identity;
            go.transform.localScale = Vector3.one;
        }

        private IEnumerator chatDisappear()
        {
            yield return new WaitForSeconds(5);
            chat.transform.parent.gameObject.SetActive(false);
            chatCoro = null;
        }
    }
}


