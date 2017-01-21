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
        private Texture2D[] handAndHealIcons = new Texture2D[2];

        public string UserName { set { userName.text = value; } }
        public Team Team { set { teamBG.texture = teamBGs[(int)value]; } }
        public string HeroName { set { heroName.text = value; } }
        public bool IsReady { set { readyIcon.enabled = value; } }
    }
}


