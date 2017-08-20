using UnityEngine;
using System.Collections;
using Framework.UI;
using System;
using UnityEngine.UI;
using System.Collections.Generic;
using Framework.AssetBundle;
using UnityEngine.SceneManagement;

namespace AGrail
{
    public class RoleChoose31 : WindowsBase
    {
        [SerializeField]
        private Transform root;
        [SerializeField]
        private List<GameObject> heros;
        [SerializeField]
        private List<GameObject> seats;

        public override WindowType Type
        {
            get
            {
                return WindowType.RoleChoose31;
            }
        }

        public override void Awake()
        {
            if (Lobby.Instance.SelectRoom.max_player != 6)
            {
                Destroy(seats[0].gameObject);
                Destroy(seats[1].gameObject);
                seats.RemoveAt(0);
                seats.RemoveAt(0);
            }

            var idx = BattleData.Instance.PlayerIdxOrder.IndexOf((int)BattleData.Instance.StartPlayerID);
            for (int i = 0; i < seats.Count; i++)
            {

                var player = BattleData.Instance.GetPlayerInfo((uint)BattleData.Instance.PlayerIdxOrder[(i + idx) % seats.Count]);
                if (player.id == BattleData.Instance.PlayerID)
                    seats[i].GetComponent<Image>().sprite =
                        AssetBundleManager.Instance.LoadAsset<Sprite>("lobby_texture", "SeatMain");
                else
                    seats[i].GetComponent<Image>().sprite =
                        (player.team == (uint)Team.Blue) ?
                            AssetBundleManager.Instance.LoadAsset<Sprite>("lobby_texture", "SeatBlue") :
                            AssetBundleManager.Instance.LoadAsset<Sprite>("lobby_texture", "SeatRed");
            }

            for (int i = 0; i < 3; i++)
            {
                var roleID = RoleChoose.Instance.RoleIDs[i];
                var sprite = AssetBundleManager.Instance.LoadAsset<Sprite>("hero_m", roleID.ToString() + "M");
                if(sprite != null)                    
                    heros[i].GetComponent<Image>().sprite = sprite;
                heros[i].GetComponent<Button>().onClick.AddListener(() =>
                {
                    RoleChoose.Instance.Choose(roleID);
                    SceneManager.LoadScene(2);
                });
            }
            base.Awake();
        }

        public override void OnDestroy()
        {
            base.OnDestroy();
        }
    }
}


