using UnityEngine;
using System.Collections;
using Framework.UI;
using System.Linq;
using UnityEngine.UI;
using System.Collections.Generic;
using Framework.AssetBundle;
using UnityEngine.SceneManagement;
using Framework.Message;

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
        [SerializeField]
        private Text info;

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
                Destroy(seats[5].gameObject);
                Destroy(seats[4].gameObject);
                seats.RemoveAt(5);
                seats.RemoveAt(4);
            }

            var idx = BattleData.Instance.PlayerIdxOrder.IndexOf((int)BattleData.Instance.StartPlayerID);
            for (int i = 0; i < seats.Count; i++)
            {
                var player = BattleData.Instance.GetPlayerInfo((uint)BattleData.Instance.PlayerIdxOrder[(i + idx) % seats.Count]);
                if (player.id == BattleData.Instance.PlayerID)
                    seats[i].GetComponent<Image>().sprite = AssetBundleManager.Instance.LoadAsset<Sprite>("lobby_texture", "SeatMain");
                seats[i].transform.GetChild(0).GetComponent<Image>().sprite = (player.team == (uint)Team.Blue) ?
                    AssetBundleManager.Instance.LoadAsset<Sprite>("lobby_texture", "SeatBlue") :
                    AssetBundleManager.Instance.LoadAsset<Sprite>("lobby_texture", "SeatRed");
            }

            for (int i = 0; i < 3; i++)
            {
                var roleID = RoleChoose.Instance.RoleIDs[i];
                var sprite = AssetBundleManager.Instance.LoadAsset<Sprite>("hero_m", roleID.ToString() + "M");
                if (sprite != null)
                    heros[i].GetComponent<Image>().sprite = sprite;
                var heroIdx = i;
                heros[i].GetComponent<Button>().onClick.AddListener(() =>
                {
                    RoleChoose.Instance.Choose(roleID);
                    for (int j = 0; j < heros.Count; j++)
                    {
                        if (heroIdx != j)
                            heros[j].SetActive(false);
                        else
                            heros[j].GetComponent<Button>().interactable = false;
                    }
                    info.text = "等待他人选择角色";
                    StartCoroutine(waitOthers());
                });
            }

            MessageSystem<MessageType>.Regist(MessageType.GameStart, this);

            base.Awake();
        }

        public override void OnDestroy()
        {
            MessageSystem<MessageType>.UnRegist(MessageType.GameStart, this);
            base.OnDestroy();
        }

        public override void OnEventTrigger(MessageType eventType, params object[] parameters)
        {
            switch (eventType)
            {
                case MessageType.GameStart:
                    if (coro != null)
                        StopCoroutine(coro);
                    SceneManager.LoadScene(2);
                    break;
            }
            base.OnEventTrigger(eventType, parameters);
        }

        private Coroutine coro = null;
        private IEnumerator waitOthers()
        {
            while (true)
            {
                yield return new WaitForSeconds(1);
                if (info.text.Count(c => { return c == '.'; }) >= 3)
                    info.text = "等待他人选择角色";
                else
                    info.text += ".";
            }
        }
    }
}


