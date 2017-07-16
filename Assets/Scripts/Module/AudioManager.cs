using Framework;
using Framework.Message;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using DG.Tweening;

namespace AGrail
{
    /// <summary>
    /// 反正音效都很简单，这个类就凑合一下吧
    /// </summary>
    public class AudioManager : MonoBehaviour, IMessageListener<MessageType>
    {
        public static AudioManager Instance { private set; get; }

        [SerializeField]
        private List<AudioClip> audioClips = new List<AudioClip>();

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
                        playAudio(audioClips[(Card.GetCard(cardMsg.card_ids[0]).Type == Card.CardType.attack) ? 0 : 2]);
                    break;
                case MessageType.PlayerHealChange:
                    playAudio(audioClips[1]);                    
                    break;
                case MessageType.HITMSG:
                    playAudio(audioClips[3]);
                    break;
                case MessageType.HURTMSG:
                    playAudio(audioClips[4]);
                    break;
                case MessageType.MoraleChange:
                    playAudio(audioClips[5]);
                    break;
                case MessageType.GemChange:
                case MessageType.CrystalChange:
                    playAudio(audioClips[6]);
                    break;
                case MessageType.ChooseRole:
                    playAudio(audioClips[7]);
                    break;
                case MessageType.TURNBEGIN:
                    var tb = parameters[0] as network.TurnBegin;
                    if (tb.idSpecified && tb.id == BattleData.Instance.PlayerID)
                    {
                        playAudio(audioClips[7]);
                        break;
                    }                    
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
