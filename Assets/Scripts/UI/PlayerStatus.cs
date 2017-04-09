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
        private Image border;
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

        public string UserName { set { userName.text = value; } }
        public Team Team { set { teamBG.texture = teamBGs[(int)value]; } }
        public string HeroName { set { heroName.text = value; } }
        public bool IsReady { set { readyIcon.enabled = value; } }
        public KeyValuePair<uint, uint> Energy
        {
            set
            {
                for (int i = 0; i < energy.childCount; i++)
                    Destroy(energy.GetChild(i).gameObject);
                for(int i = 0; i < value.Key; i++)
                {
                    var go = new GameObject();
                    go.transform.parent = energy;
                    go.AddComponent<RawImage>().texture = energeIcons[0];
                }
                for(int i = 0; i < value.Value; i++)
                {
                    var go = new GameObject();
                    go.transform.parent = energy;
                    go.AddComponent<RawImage>().texture = energeIcons[1];
                }
            }
        }
        public List<uint> BasicAndExCards
        {
            set
            {
                //for (int i = 0; i < basicAndExCards.childCount; i++)
                //    Destroy(basicAndExCards.GetChild(i).gameObject);
                //foreach(var v in value)
                //{

                //}
            }
        }

    }
}


