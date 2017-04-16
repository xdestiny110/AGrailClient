using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace AGrail
{
    public class PlayerStatus : MonoBehaviour
    {
        [SerializeField]
        private Text userName;
        [SerializeField]
        private RawImage teamBG;
        [SerializeField]
        private RawImage heroIcon;
        [SerializeField]
        private RawImage heroProperty;
        [SerializeField]
        private Text heroName;
        [SerializeField]
        private Transform energy;
        [SerializeField]
        private Transform handCard;
        [SerializeField]
        private Transform heal;
        [SerializeField]
        private Transform basicAndExCards;
        [SerializeField]
        private Image turnBorder;
        [SerializeField]
        private Image selectBorder;
        [SerializeField]
        private Image readyIcon;

        [SerializeField]
        private Texture2D[] teamBGs = new Texture2D[3];
        [SerializeField]
        private Texture2D[] healIcons = new Texture2D[3];
        [SerializeField]
        private Texture2D[] properties = new Texture2D[5];
        [SerializeField]
        private Texture2D[] energeIcons = new Texture2D[2];
        [SerializeField]
        private GameObject tokenPrefab;

        private RoleBase role;
        private Transform token0, token1, token2;

        public string UserName { set { userName.text = value; } }
        public Team Team { set { teamBG.texture = teamBGs[(int)value]; } }        
        public bool IsReady { set { readyIcon.enabled = value; } }        
        
        public uint RoleID
        {
            set
            {
                role = RoleFactory.Create(value);
                heroName.text = role.RoleName;
                heroProperty.texture = properties[(int)role.RoleProperty];
                if (role.HasYellow)
                {
                    var go = Instantiate(tokenPrefab);
                    go.GetComponent<RawImage>().texture = Resources.Load<Texture2D>("Icons/token0");
                    go.name = "token0";
                    go.transform.SetParent(basicAndExCards);
                    go.transform.GetChild(0).GetComponent<Text>().text = "0";
                    token0 = go.transform;
                }
                if (role.HasBlue)
                {
                    var go = Instantiate(tokenPrefab);
                    go.GetComponent<RawImage>().texture = Resources.Load<Texture2D>("Icons/token1");
                    go.name = "token1";
                    go.transform.SetParent(basicAndExCards);
                    go.transform.GetChild(0).GetComponent<Text>().text = "0";
                    token1 = go.transform;
                }
                if (role.HasCoverd)
                {
                    var go = Instantiate(tokenPrefab);
                    go.GetComponent<RawImage>().texture = Resources.Load<Texture2D>("Icons/token2");
                    go.name = "token2";
                    go.transform.SetParent(basicAndExCards);
                    go.transform.GetChild(0).GetComponent<Text>().text = "0";
                    token2 = go.transform;
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

        public KeyValuePair<uint, uint> Energy
        {
            set
            {
                for (int i = 0; i < energy.childCount; i++)
                    Destroy(energy.GetChild(i).gameObject);
                for(int i = 0; i < value.Key; i++)
                {
                    var go = new GameObject();
                    go.transform.SetParent(energy);
                    go.AddComponent<RawImage>().texture = energeIcons[0];                    
                }
                for(int i = 0; i < value.Value; i++)
                {
                    var go = new GameObject();
                    go.transform.SetParent(energy);
                    go.AddComponent<RawImage>().texture = energeIcons[1];
                }
            }
        }

        public void Token(uint yellow, uint blue, uint coverd)
        {
            if (token0 != null)
                token0.GetChild(0).GetComponent<Text>().text = yellow.ToString();
            if (token1 != null)
                token1.GetChild(0).GetComponent<Text>().text = blue.ToString();
            if (token2 != null)
                token2.GetChild(0).GetComponent<Text>().text = coverd.ToString();
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
                    var card = new Card(v);

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
                    else
                        Debug.LogErrorFormat("Basic card is error! CardID = {0}", v);
                    var go = new GameObject();
                    go.transform.SetParent(basicAndExCards);
                    go.AddComponent<RawImage>().texture = icon;
                }
            }
        }

        public KeyValuePair<uint, uint> HandCount
        {
            set
            {
                for (int i = 0; i < handCard.childCount; i++)
                    Destroy(handCard.GetChild(i).gameObject);
                for(int i = 0;i<Mathf.Min(value.Key, value.Value); i++)
                {
                    var go = new GameObject();
                    go.transform.SetParent(handCard);
                    go.AddComponent<RawImage>().texture = healIcons[1];
                }
                for (int i = 0; i < (int)value.Value - (int)value.Key; i++)
                {
                    var go = new GameObject();
                    go.transform.SetParent(handCard);
                    go.AddComponent<RawImage>().texture = healIcons[0];
                }
                for (int i = 0; i < (int)value.Key - (int)value.Value; i++)
                {
                    var go = new GameObject();
                    go.transform.SetParent(handCard);
                    var ri = go.AddComponent<RawImage>();
                    ri.texture = healIcons[0];
                    ri.color = Color.red;
                }
            }
        }

        public uint HealCount
        {
            set
            {
                for (int i = 0; i < heal.childCount; i++)
                    Destroy(heal.GetChild(i).gameObject);
                for(int i = 0; i < value; i++)
                {
                    var go = new GameObject();
                    go.transform.SetParent(heal);
                    go.AddComponent<RawImage>().texture = healIcons[2];
                }
            }
        }

    }
}


