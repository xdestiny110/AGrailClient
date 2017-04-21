using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;

namespace AGrail
{
    public class PlayerStatusQT : MonoBehaviour
    {

        [SerializeField]
        private Transform teamBG;
        [SerializeField]
        private Text playerName;
        [SerializeField]
        private Text playerID;
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

        public uint PlayerID
        {
            set
            {
                playerID.text = value.ToString();
            }
        }

        public uint RoleID
        {
            set
            {
                role = RoleFactory.Create(value);
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
                    go.transform.localPosition = Vector3.zero;
                    go.transform.localRotation = Quaternion.identity;
                    go.transform.localScale = Vector3.one;
                    go.AddComponent<RawImage>().texture = icon;
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
                    var go = new GameObject();
                    go.transform.SetParent(energy);
                    go.transform.localPosition = Vector3.zero;
                    go.transform.localRotation = Quaternion.identity;
                    go.transform.localScale = Vector3.one;
                    go.AddComponent<RawImage>().texture = Resources.Load<Texture2D>("Icons/gem");
                }
                for (int i = 0; i < value.Value; i++)
                {
                    var go = new GameObject();
                    go.transform.SetParent(energy);
                    go.transform.localPosition = Vector3.zero;
                    go.transform.localRotation = Quaternion.identity;
                    go.transform.localScale = Vector3.one;
                    go.AddComponent<RawImage>().texture = Resources.Load<Texture2D>("Icons/crystal");
                }
            }
        }

        public KeyValuePair<uint, uint> HandCount
        {
            set
            {
                for (int i = 0; i < handCard.childCount; i++)
                    Destroy(handCard.GetChild(i).gameObject);
                for (int i = 0; i < Mathf.Min(value.Key, value.Value); i++)
                {
                    var go = new GameObject();
                    go.transform.SetParent(handCard);
                    go.transform.localPosition = Vector3.zero;
                    go.transform.localRotation = Quaternion.identity;
                    go.transform.localScale = Vector3.one;
                    go.AddComponent<RawImage>().texture = Resources.Load<Texture2D>("Icons/UI6-22");
                }
                for (int i = 0; i < (int)value.Value - (int)value.Key; i++)
                {
                    var go = new GameObject();
                    go.transform.SetParent(handCard);
                    go.transform.localPosition = Vector3.zero;
                    go.transform.localRotation = Quaternion.identity;
                    go.transform.localScale = Vector3.one;
                    go.AddComponent<RawImage>().texture = Resources.Load<Texture2D>("Icons/UI6-23");
                }
                for (int i = 0; i < (int)value.Key - (int)value.Value; i++)
                {
                    var go = new GameObject();
                    go.transform.SetParent(handCard);
                    go.transform.localPosition = Vector3.zero;
                    go.transform.localRotation = Quaternion.identity;
                    go.transform.localScale = Vector3.one;
                    var ri = go.AddComponent<RawImage>();
                    ri.texture = Resources.Load<Texture2D>("Icons/UI6-23");
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
                for (int i = 0; i < value; i++)
                {
                    var go = new GameObject();
                    go.transform.SetParent(heal);
                    go.transform.localPosition = Vector3.zero;
                    go.transform.localRotation = Quaternion.identity;
                    go.transform.localScale = Vector3.one;
                    go.AddComponent<RawImage>().texture = Resources.Load<Texture2D>("Icons/UI6-24");
                }
            }
        }
    }
}

