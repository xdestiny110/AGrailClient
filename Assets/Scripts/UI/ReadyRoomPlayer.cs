using Framework.AssetBundle;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace AGrail
{
    public class ReadyRoomPlayer : MonoBehaviour
    {
        [SerializeField]
        private Text playerName;
        [SerializeField]
        private Image readyIcon;
        [SerializeField]
        private Image teamBG;

        public string PlayerName
        {
            set
            {
                playerName.text = value;
            }
        }

        public bool IsReady
        {
            set
            {
                readyIcon.enabled = value;
            }
        }

        public Team Team
        {
            set
            {
                teamBG.sprite = AssetBundleManager.Instance.LoadAsset<Sprite>("ready_texture", "Team" + value.ToString());
            }
        }

        public void Reset()
        {
            Team = Team.Other;
            IsReady = false;
            PlayerName = "";
        }

        public void OnClick()
        {
            BattleData.Instance.ChooseTeam();
        }
    }
}


