using Framework.AssetBundle;
using Framework.Message;
using System.Collections.Generic;
using UnityEngine;

namespace AGrail
{
    /// <summary>
    /// 反正音效都很简单，这个类就凑合一下吧
    /// </summary>
    public class AudioManager : MonoBehaviour, IMessageListener<MessageType>
    {
        public static AudioManager Instance { private set; get; }
        private Queue<AudioSource> sources = new Queue<AudioSource>();

        void Awake()
        {
            Instance = this;            
            DontDestroyOnLoad(this);

            MessageSystem<MessageType>.Regist(MessageType.CARDMSG, this);
            MessageSystem<MessageType>.Regist(MessageType.HITMSG, this);
            MessageSystem<MessageType>.Regist(MessageType.PlayerHealChange, this);            
            MessageSystem<MessageType>.Regist(MessageType.HURTMSG, this);
            MessageSystem<MessageType>.Regist(MessageType.MoraleChange, this);
            MessageSystem<MessageType>.Regist(MessageType.GemChange, this);
            MessageSystem<MessageType>.Regist(MessageType.CrystalChange, this);
            MessageSystem<MessageType>.Regist(MessageType.TURNBEGIN, this);
            MessageSystem<MessageType>.Regist(MessageType.ChooseRole, this);
        }

        void Update()
        {
            if(sources.Count > 0 && !sources.Peek().isPlaying)            
                Destroy(sources.Dequeue());            
        }

        public void OnEventTrigger(MessageType eventType, params object[] parameters)
        {
            switch (eventType)
            {
                case MessageType.CARDMSG:
                    var cardMsg = parameters[0] as network.CardMsg;
                    if (cardMsg.dst_idSpecified)
                    {
                        var card = Card.GetCard(cardMsg.card_ids[0]);
                        if(card.Type == Card.CardType.attack)                        
                            playAudio(AssetBundleManager.Instance.LoadAsset<AudioClip>("audio", "atk-" + card.Element.ToString()));
                        else
                            playAudio(AssetBundleManager.Instance.LoadAsset<AudioClip>("audio", "spell-" + card.Name.ToString()));
                    }                        
                    break;
                case MessageType.PlayerHealChange:
                    playAudio(AssetBundleManager.Instance.LoadAsset<AudioClip>("audio", "sys-heal"));
                    break;
                case MessageType.HITMSG:
                    playAudio(AssetBundleManager.Instance.LoadAsset<AudioClip>("audio", "sys-hit"));
                    break;
                case MessageType.HURTMSG:
                    playAudio(AssetBundleManager.Instance.LoadAsset<AudioClip>("audio", "sys-hurt"));
                    break;
                case MessageType.MoraleChange:
                    playAudio(AssetBundleManager.Instance.LoadAsset<AudioClip>("audio", "sys-morale"));
                    break;
                case MessageType.GemChange:
                case MessageType.CrystalChange:
                    playAudio(AssetBundleManager.Instance.LoadAsset<AudioClip>("audio", "sys-energy"));
                    break;
                case MessageType.ChooseRole:
                    playAudio(AssetBundleManager.Instance.LoadAsset<AudioClip>("audio", "sys-turn"));
                    break;
                case MessageType.TURNBEGIN:
                    var tb = parameters[0] as network.TurnBegin;
                    if (tb.idSpecified)                    
                        playAudio(AssetBundleManager.Instance.LoadAsset<AudioClip>("audio", BattleData.Instance.GetPlayerInfo(tb.id).role_id.ToString()));
                    break;
            }
        }

        private void playAudio(AudioClip clip)
        {
            var source = gameObject.AddComponent<AudioSource>();
            source.clip = clip;
            source.Play();
            sources.Enqueue(source);
        }        
    }
}
