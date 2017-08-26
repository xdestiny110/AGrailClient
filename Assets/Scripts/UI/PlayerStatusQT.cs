using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;
using Framework.Message;

namespace AGrail
{
    public class PlayerStatusQT : MonoBehaviour, IMessageListener<MessageType>
    {
        [SerializeField]
        private Transform teamBG;
        [SerializeField]
        private Text playerName;
        [SerializeField]
        private Text txtPlayerID;
        [SerializeField]
        private RawImage hero;
        [SerializeField]
        private Transform handCard;
        [SerializeField]
        private Transform heal;
        [SerializeField]
        private Transform basicAndExCards;
        [SerializeField]
        private Transform energy;
        [SerializeField]
        private Image turnBorder;
        [SerializeField]
        private Image selectBorder;
        [SerializeField]
        private RawImage kneltIcon;
        [SerializeField]
        private Image readyIcon;
        [SerializeField]
        private GameObject tokenPrefab;
        [SerializeField]
        private GameObject handCardPrefab;
        [SerializeField]
        private GameObject energyPrefab;
        [SerializeField]
        private Button btnPlayer;

        private RoleBase role;
        private Text token0, token1, token2;

        public Team Team
        {
            set
            {
                for (int i = 0; i < teamBG.childCount; i++)
                    teamBG.GetChild(i).gameObject.SetActive(false);
                teamBG.GetChild((int)value).gameObject.SetActive(true);
            }
        }

        public bool IsReady
        {
            set
            {
                readyIcon.enabled = value;
            }
        }

        public string PlayerName
        {
            set
            {
                playerName.text = value;
            }
        }

        private uint playerID;
        public uint PlayerID
        {
            set
            {
                playerID = value;
                txtPlayerID.text = value.ToString();
            }
            get
            {
                return playerID;
            }
        }

        public uint RoleID
        {
            set
            {
                role = RoleFactory.Create(value);
                hero.enabled = true;
                readyIcon.enabled = false;
                hero.texture = Resources.Load<Texture2D>("Hero/" + value.ToString());
                if (role.HasYellow)
                {
                    var go = Instantiate(tokenPrefab);
                    go.GetComponent<RawImage>().texture = Resources.Load<Texture2D>("Icons/token0");
                    go.name = "token0";
                    go.transform.SetParent(basicAndExCards);
                    go.transform.localPosition = tokenPrefab.transform.localPosition;
                    go.transform.localRotation = tokenPrefab.transform.localRotation;
                    go.transform.localScale = tokenPrefab.transform.localScale;
                    token0 = go.transform.GetChild(0).GetComponent<Text>();
                    token0.text = "0";
                }
                if (role.HasBlue)
                {
                    var go = Instantiate(tokenPrefab);
                    go.GetComponent<RawImage>().texture = Resources.Load<Texture2D>("Icons/token1");
                    go.name = "token1";
                    go.transform.SetParent(basicAndExCards);
                    go.transform.localPosition = tokenPrefab.transform.localPosition;
                    go.transform.localRotation = tokenPrefab.transform.localRotation;
                    go.transform.localScale = tokenPrefab.transform.localScale;
                    token1 = go.transform.GetChild(0).GetComponent<Text>();
                    token1.text = "0";
                }
                if (role.HasCoverd)
                {
                    var go = Instantiate(tokenPrefab);
                    go.GetComponent<RawImage>().texture = Resources.Load<Texture2D>("Icons/token2");
                    go.name = "token2";
                    go.transform.SetParent(basicAndExCards);
                    go.transform.localPosition = tokenPrefab.transform.localPosition;
                    go.transform.localRotation = tokenPrefab.transform.localRotation;
                    go.transform.localScale = tokenPrefab.transform.localScale;
                    token2 = go.transform.GetChild(0).GetComponent<Text>();
                    token2.text = "0";
                }
            }
        }

        public bool IsTurn
        {
            set
            {
                turnBorder.enabled = value;
            }
        }

        public bool Knelt
        {
            set
            {
                if (value)
                {
                    kneltIcon.texture = Resources.Load<Texture2D>("Icons/" + role.Knelt);
                    kneltIcon.enabled = true;
                }
                else
                    kneltIcon.enabled = false;
            }
        }

        public uint YellowToken
        {
            set
            {
                if(token0 != null)
                    token0.text = value.ToString();
            }
        }

        public uint BlueToken
        {
            set
            {
                if(token1 != null)
                    token1.text = value.ToString();
            }
        }

        public uint Covered
        {
            set
            {
                if(token2 != null)
                    token2.text = value.ToString();
            }
        }

        public List<uint> BasicAndExCards
        {
            set
            {
                for (int i = 0; i < basicAndExCards.childCount; i++)
                {
                    if (basicAndExCards.GetChild(i).name.StartsWith("token")) continue;
                    Destroy(basicAndExCards.GetChild(i).gameObject);
                }

                foreach (var v in value)
                {
                    Texture2D icon = new Texture2D(0, 0);
                    var card = Card.GetCard(v);

                    if (card.Name == Card.CardName.中毒)
                        icon = Resources.Load<Texture2D>("Icons/du");
                    else if (card.Name == Card.CardName.圣盾 || card.HasSkill("天使之墙"))
                        icon = Resources.Load<Texture2D>("Icons/dun");
                    else if (card.Name == Card.CardName.虚弱)
                        icon = Resources.Load<Texture2D>("Icons/xu");
                    else if (card.HasSkill("威力赐福"))
                        icon = Resources.Load<Texture2D>("Icons/wei");
                    else if (card.HasSkill("迅捷赐福"))
                        icon = Resources.Load<Texture2D>("Icons/xun");
                    else if (card.HasSkill("地之封印"))
                        icon = Resources.Load<Texture2D>("Icons/diFeng");
                    else if (card.HasSkill("火之封印"))
                        icon = Resources.Load<Texture2D>("Icons/huoFeng");
                    else if (card.HasSkill("水之封印"))
                        icon = Resources.Load<Texture2D>("Icons/shuiFeng");
                    else if (card.HasSkill("风之封印"))
                        icon = Resources.Load<Texture2D>("Icons/fengFeng");
                    else if (card.HasSkill("雷之封印"))
                        icon = Resources.Load<Texture2D>("Icons/leiFeng");
                    else if (card.Name == Card.CardName.五行束缚)
                        icon = Resources.Load<Texture2D>("Icons/ShuFu");
                    else if (card.Name == Card.CardName.挑衅)
                        icon = Resources.Load<Texture2D>("Icons/TiaoXin");
                    else if (card.Name == Card.CardName.灵魂链接)
                        icon = Resources.Load<Texture2D>("Icons/LianJie");
                    else if (card.Name == Card.CardName.同生共死)
                        icon = Resources.Load<Texture2D>("Icons/TongSheng");
                    else if (card.Name == Card.CardName.永恒乐章)
                        icon = Resources.Load<Texture2D>("Icons/YueZhang");
                    else
                        Debug.LogErrorFormat("Basic card is error! CardID = {0}", v);
                    var go = new GameObject();
                    go.transform.SetParent(basicAndExCards);
                    go.transform.localPosition = Vector3.zero;
                    go.transform.localRotation = Quaternion.identity;
                    go.transform.localScale = Vector3.one;
                    go.AddComponent<Image>().sprite = Sprite.Create(icon, new Rect(0, 0, icon.width, icon.height), Vector2.zero);
                }
            }
        }

        public KeyValuePair<uint, uint> Energy
        {
            set
            {
                for (int i = 0; i < energy.childCount; i++)
                    Destroy(energy.GetChild(i).gameObject);
                for (int i = 0; i < value.Key; i++)
                {
                    var go = Instantiate(energyPrefab);
                    go.transform.SetParent(energy);
                    go.transform.localPosition = Vector3.zero;
                    go.transform.localRotation = Quaternion.identity;
                    go.transform.localScale = Vector3.one;
                    go.transform.Find("gem").gameObject.SetActive(true);
                }
                for (int i = 0; i < value.Value; i++)
                {
                    var go = Instantiate(energyPrefab);
                    go.transform.SetParent(energy);
                    go.transform.localPosition = Vector3.zero;
                    go.transform.localRotation = Quaternion.identity;
                    go.transform.localScale = Vector3.one;
                    go.transform.Find("crystal").gameObject.SetActive(true);
                }
            }
        }

        public KeyValuePair<uint, uint> HandCount
        {
            set
            {
                MessageSystem<MessageType>.Notify(MessageType.LogChange);
                for (int i = 0; i < handCard.childCount; i++)
                    Destroy(handCard.GetChild(i).gameObject);
                for (int i = 0; i < Mathf.Min(value.Key, value.Value); i++)
                {
                    var go = Instantiate(handCardPrefab);
                    go.transform.SetParent(handCard);
                    go.transform.localPosition = Vector3.zero;
                    go.transform.localRotation = Quaternion.identity;
                    go.transform.localScale = Vector3.one;
                    go.transform.Find("HandCard").gameObject.SetActive(true);
                }
                for (int i = 0; i < (int)value.Value - (int)value.Key; i++)
                {
                    var go = Instantiate(handCardPrefab);
                    go.transform.SetParent(handCard);
                    go.transform.localPosition = Vector3.zero;
                    go.transform.localRotation = Quaternion.identity;
                    go.transform.localScale = Vector3.one;
                    go.transform.Find("Empty").gameObject.SetActive(true);
                }
                for (int i = 0; i < (int)value.Key - (int)value.Value; i++)
                {
                    var go = Instantiate(handCardPrefab);
                    go.transform.SetParent(handCard);
                    go.transform.localPosition = Vector3.zero;
                    go.transform.localRotation = Quaternion.identity;
                    go.transform.localScale = Vector3.one;
                    var empty = go.transform.Find("Empty").gameObject;
                    empty.SetActive(true);
                    empty.GetComponent<Image>().color = Color.red;
                }
            }
        }

        public uint HealCount
        {
            set
            {
                for (int i = 0; i < heal.childCount; i++)
                    Destroy(heal.GetChild(i).gameObject);
                for (int i = 0; i < value; i++)
                {
                    var go = Instantiate(handCardPrefab);
                    go.transform.SetParent(heal);
                    go.transform.localPosition = Vector3.zero;
                    go.transform.localRotation = Quaternion.identity;
                    go.transform.localScale = Vector3.one;
                    go.transform.Find("Heal").gameObject.SetActive(true);
                }
            }
        }

        public bool IsEnable
        {
            get { return btnPlayer.interactable; }
            set {
                btnPlayer.interactable = value;
            }
        }

        void Awake()
        {
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
                    if (BattleData.Instance.Agent.SelectPlayers.Contains(playerID))
                        selectBorder.enabled = true;
                    else
                        selectBorder.enabled = false;
                    break;
            }
        }

        public void AddBtnPlayerCallback(uint id)
        {
            btnPlayer.onClick.AddListener(()=>
            {
                if (!selectBorder.enabled)
                    BattleData.Instance.Agent.AddSelectPlayer(id);
                else
                    BattleData.Instance.Agent.RemoveSelectPlayer(id);
            });
            btnPlayer.interactable = false;
            selectBorder.enabled = false;
        }
    }
}

