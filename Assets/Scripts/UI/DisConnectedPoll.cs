using Framework.UI;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;
using Framework.AssetBundle;
using Framework.Network;
using Framework.Message;
using network;

namespace AGrail
{
    public class DisConnectedPoll : WindowsBase
    {
        [SerializeField]
        private Transform PlayerIcosRoot;
        [SerializeField]
        private Text Text;
        [SerializeField]
        private Button Wait;
        [SerializeField]
        private Button GiveUp;
        public override WindowType Type
        {
            get
            {
                return WindowType.DisConnectedPoll;
            }
        }

        public override object[] Parameters
        {
            get
            {
                return base.Parameters;
            }
            
            set
            {
                base.Parameters = value;
                var prefab = AssetBundleManager.Instance.LoadAsset("battle", "PlayerIco");
                foreach (var v in BattleData.Instance.DisConnectedPlayer)
                {
                    var go = Instantiate(prefab);
                    go.transform.parent = PlayerIcosRoot;
                    go.transform.localPosition = Vector3.zero;
                    go.transform.localRotation = Quaternion.identity;
                    go.transform.localScale = Vector3.one;
                    go.GetComponent<PlayerIco>().IcoID = v;
                    Text.text= "<color=#ff0000>" + BattleData.Instance.GetPlayerInfo(v).nickname + "</color>　"+ Text.text;
                }
            }
        }
        public override void Awake()
        {
            Wait.onClick.AddListener(onBtnWaitClick);
            GiveUp.onClick.AddListener(onBtnGiveUpClick);
            base.Awake();
        }
        private void onBtnWaitClick()
        {
            PollingResponse proto = new PollingResponse() { option = 0 };
            GameManager.TCPInstance.Send(new Protobuf() { Proto = proto, ProtoID = ProtoNameIds.POLLINGRESPONSE });
            MessageSystem<Framework.Message.MessageType>.Notify(Framework.Message.MessageType.SendHint, "已投票，请稍等");
            GameManager.UIInstance.PopWindow(Framework.UI.WinMsg.None);
        }
        private void onBtnGiveUpClick()
        {
            PollingResponse proto = new PollingResponse() { option = 1 };
            GameManager.TCPInstance.Send(new Protobuf() { Proto = proto, ProtoID = ProtoNameIds.POLLINGRESPONSE });
            MessageSystem<Framework.Message.MessageType>.Notify(Framework.Message.MessageType.SendHint, "已投票，请稍等");
            GameManager.UIInstance.PopWindow(Framework.UI.WinMsg.None);
        }

    }
}
