using Framework.Message;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Framework.AssetBundle;

namespace AGrail
{
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
        private RawImage hero;
        [SerializeField]
        private RawImage knelt;
        [SerializeField]
        private Text playerName;
        [SerializeField]
        private Transform handArea;
        [SerializeField]
        private Transform healArea;
        [SerializeField]
        private Transform energyArea;
        [SerializeField]
        private Transform basicAndExCardArea;
        [SerializeField]
        private Transform animationPos;

        private Button btnPlayer;
        private RoleBase role;
        private Text token0, token1, token2;

        public uint ID { set; get; }

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
                if (ID == BattleData.Instance.MainPlayer.id || (ID == BattleData.Instance.PlayerInfos[0].id && BattleData.Instance.PlayerID == 9))
                    hero.texture = AssetBundleManager.Instance.LoadAsset<Texture2D>("hero_l", value.ToString() + "L");
                else
                    hero.texture = AssetBundleManager.Instance.LoadAsset<Texture2D>("hero_s", value.ToString() + "S");
                if(hero.texture == null)
                {
                    if (ID == BattleData.Instance.MainPlayer.id || (ID == BattleData.Instance.PlayerInfos[0].id && BattleData.Instance.PlayerID == 9))
                        hero.texture = AssetBundleManager.Instance.LoadAsset<Texture2D>("hero_l", "0L");
                    else
                        hero.texture = AssetBundleManager.Instance.LoadAsset<Texture2D>("hero_s", "0S");
                }
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
                    knelt.texture = Resources.Load<Texture2D>("Icons/" + role.Knelt);
                    knelt.enabled = true;
                }
                else
                    knelt.enabled = false;
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
                    GameObject prefab = null;

                    if (card.Name == Card.CardName.中毒)
                        prefab = AssetBundleManager.Instance.LoadAsset("battle", "du");
                    else if (card.Name == Card.CardName.圣盾 || card.HasSkill("天使之墙"))
                        prefab = AssetBundleManager.Instance.LoadAsset("battle", "dun");
                    else if (card.Name == Card.CardName.虚弱)
                        prefab = AssetBundleManager.Instance.LoadAsset("battle", "xu");
                    else if (card.HasSkill("威力赐福"))
                        prefab = AssetBundleManager.Instance.LoadAsset("battle", "wei");
                    else if (card.HasSkill("迅捷赐福"))
                        prefab = AssetBundleManager.Instance.LoadAsset("battle", "xun");
                    else if (card.HasSkill("地之封印"))
                        prefab = AssetBundleManager.Instance.LoadAsset("battle", "difeng");
                    else if (card.HasSkill("火之封印"))
                        prefab = AssetBundleManager.Instance.LoadAsset("battle", "huofeng");
                    else if (card.HasSkill("水之封印"))
                        prefab = AssetBundleManager.Instance.LoadAsset("battle", "shuifeng");
                    else if (card.HasSkill("风之封印"))
                        prefab = AssetBundleManager.Instance.LoadAsset("battle", "fengfeng");
                    else if (card.HasSkill("雷之封印"))
                        prefab = AssetBundleManager.Instance.LoadAsset("battle", "leifeng");
                    else if (card.Name == Card.CardName.五行束缚)
                        prefab = AssetBundleManager.Instance.LoadAsset("battle", "shufu");
                    else if (card.Name == Card.CardName.挑衅)
                        prefab = AssetBundleManager.Instance.LoadAsset("battle", "tiaoxin");
                    else if (card.Name == Card.CardName.灵魂链接)
                        prefab = AssetBundleManager.Instance.LoadAsset("battle", "lianjie");
                    else if (card.Name == Card.CardName.同生共死)
                        prefab = AssetBundleManager.Instance.LoadAsset("battle", "tongsheng");
                    else if (card.Name == Card.CardName.永恒乐章)
                        prefab = AssetBundleManager.Instance.LoadAsset("battle", "yuezhang");
                    else {
                        Debug.LogErrorFormat("Basic card is error! CardID = {0}", v);
                        return;
                    }
                    addChildGO(basicAndExCardArea, prefab, 1);                
                }
            }
        }

        public KeyValuePair<uint, uint> Energy
        {
            set
            {
                for (int i = 0; i < energyArea.childCount; i++)
                    Destroy(energyArea.GetChild(i).gameObject);
                var prefab = AssetBundleManager.Instance.LoadAsset("battle", "Gem");
                addChildGO(energyArea, prefab, (int)value.Key);
                prefab = AssetBundleManager.Instance.LoadAsset("battle", "Crystal");
                addChildGO(energyArea, prefab, (int)value.Value);
            }
        }

        public KeyValuePair<uint, uint> HandCount
        {
            set
            {
                for (int i = 0; i < handArea.childCount; i++)
                    Destroy(handArea.GetChild(i).gameObject);
                var prefab = AssetBundleManager.Instance.LoadAsset("battle", "HandCard");
                addChildGO(handArea, prefab, (int)Mathf.Min(value.Key, value.Value));
                prefab = AssetBundleManager.Instance.LoadAsset("battle", "HandCardEmpty");
                addChildGO(handArea, prefab, (int)value.Value - (int)value.Key);                
                if((int)value.Key - (int)value.Value > 0)
                {
                    //手牌超上限
                    for (int i = 0; i < handArea.childCount; i++)
                        handArea.GetChild(i).GetComponent<Image>().color = Color.red;
                }
            }
        }

        public uint HealCount
        {
            set
            {
                for (int i = 0; i < healArea.childCount; i++)
                    Destroy(healArea.GetChild(i).gameObject);
                var prefab = AssetBundleManager.Instance.LoadAsset("battle", "Heal");
                addChildGO(healArea, prefab, (int)value);
                prefab = AssetBundleManager.Instance.LoadAsset("battle", "HandCardEmpty");
                addChildGO(healArea, prefab, (int)role.MaxHealCount - (int)value);
            }
        }

        void Awake()
        {
            btnPlayer = GetComponent<Button>();
            btnPlayer.onClick.AddListener(OnClick);
        }

        public void OnEventTrigger(MessageType eventType, params object[] parameters)
        {
            switch (eventType)
            {
                case MessageType.AgentSelectPlayer:
                    if (BattleData.Instance.Agent.SelectPlayers.Contains(ID))
                        select.enabled = true;
                    else
                        select.enabled = false;
                    break;
            }
        }

        public void OnClick()
        {
            if (!select.enabled)
                BattleData.Instance.Agent.AddSelectPlayer(ID);
            else
                BattleData.Instance.Agent.RemoveSelectPlayer(ID);
        }

        private void addChildGO(Transform parent, GameObject prefab, int cnt)
        {
            for(int i = 0; i < cnt; i++)
            {
                var go = Instantiate(prefab);
                go.transform.SetParent(parent);                
                go.transform.localPosition = Vector3.zero;
                go.transform.localRotation = Quaternion.identity;
                go.transform.localScale = Vector3.one;
            }
        }
    }
}


