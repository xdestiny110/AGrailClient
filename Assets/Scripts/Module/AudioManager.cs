using Framework.AssetBundle;
using Framework.Message;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace AGrail
{
    /// <summary>
    /// 真的是凑合一下...未来肯定要改成协程回调的方式, 轮询好傻
    /// </summary>
    public class AudioManager : MonoBehaviour, IMessageListener<MessageType>
    {
        public static AudioManager Instance { private set; get; }
        private List<AudioSource> ses = new List<AudioSource>();
        private AudioSource bgm;
        private uint turn = 0;

        public float BGMVolume
        {
            set
            {
                PlayerPrefs.SetFloat("BGM", value);
                bgm.volume = value;
            }
            get
            {
                if (!PlayerPrefs.HasKey("BGM"))
                    PlayerPrefs.SetFloat("BGM", 0.5f);
                return PlayerPrefs.GetFloat("BGM");
            }
        }

        public float SEVolume
        {
            set
            {
                PlayerPrefs.SetFloat("SE", value);
                foreach (var v in ses)
                    if (v != null)
                        v.volume = value;
            }
            get
            {
                if (!PlayerPrefs.HasKey("SE"))
                    PlayerPrefs.SetFloat("SE", 0.5f);
                return PlayerPrefs.GetFloat("SE");
            }
        }

        void Awake()
        {
            Instance = this;
            DontDestroyOnLoad(this);

            bgm = gameObject.AddComponent<AudioSource>();
            bgm.playOnAwake = false;
            bgm.volume = BGMVolume;

            MessageSystem<MessageType>.Regist(MessageType.CARDMSG, this);
            MessageSystem<MessageType>.Regist(MessageType.HITMSG, this);
            MessageSystem<MessageType>.Regist(MessageType.PlayerHealChange, this);
            MessageSystem<MessageType>.Regist(MessageType.HURTMSG, this);
            MessageSystem<MessageType>.Regist(MessageType.MoraleChange, this);
            MessageSystem<MessageType>.Regist(MessageType.GemChange, this);
            MessageSystem<MessageType>.Regist(MessageType.CrystalChange, this);
            MessageSystem<MessageType>.Regist(MessageType.TURNBEGIN, this);
            MessageSystem<MessageType>.Regist(MessageType.ChooseRole, this);
            MessageSystem<MessageType>.Regist(MessageType.PlayBGM, this);
            MessageSystem<MessageType>.Regist(MessageType.Win, this);
            MessageSystem<MessageType>.Regist(MessageType.Lose, this);
        }

        void Update()
        {
            foreach (var v in ses)
                if (v != null && !v.isPlaying) Destroy(v);
            var idx = ses.FindLastIndex(s => { return s == null; });
            while (idx >= 0)
            {
                ses.RemoveAt(idx);
                idx = ses.FindLastIndex(s => { return s == null; });
            }

            if (!bgm.isPlaying)
                playBGM(SceneManager.GetActiveScene());
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
                            playSEAudio(AssetBundleManager.Instance.LoadAsset<AudioClip>("audio", "atk-" + card.Element.ToString()));
                        else
                            playSEAudio(AssetBundleManager.Instance.LoadAsset<AudioClip>("audio", "spell-" + card.Name.ToString()));
                    }
                    break;
                case MessageType.PlayerHealChange:
                    playSEAudio(AssetBundleManager.Instance.LoadAsset<AudioClip>("audio", "sys-heal"));
                    break;
                case MessageType.HITMSG:
                    playSEAudio(AssetBundleManager.Instance.LoadAsset<AudioClip>("audio", "sys-hit"));
                    break;
                case MessageType.HURTMSG:
                    playSEAudio(AssetBundleManager.Instance.LoadAsset<AudioClip>("audio", "sys-hurt"));
                    break;
                case MessageType.MoraleChange:
                    if (turn!=0)
                        playSEAudio(AssetBundleManager.Instance.LoadAsset<AudioClip>("audio", "sys-morale"));
                    break;
                case MessageType.GemChange:
                case MessageType.CrystalChange:
                    if (turn != 0)
                        playSEAudio(AssetBundleManager.Instance.LoadAsset<AudioClip>("audio", "sys-energy"));
                    break;
                case MessageType.ChooseRole:
                    playSEAudio(AssetBundleManager.Instance.LoadAsset<AudioClip>("audio", "sys-turn"));
                    turn = 0;
                    break;
                case MessageType.TURNBEGIN:
                    var tb = parameters[0] as network.TurnBegin;
                    turn = tb.round;
                    if (tb.idSpecified)
                        playSEAudio(AssetBundleManager.Instance.LoadAsset<AudioClip>("audio", BattleData.Instance.GetPlayerInfo(tb.id).role_id.ToString()));
                    break;
                case MessageType.PlayBGM:
                    playBGM(SceneManager.GetActiveScene());
                    break;
                case MessageType.Win:
                    playBGM(SceneManager.GetActiveScene(), true);
                    break;
                case MessageType.Lose:
                    playBGM(SceneManager.GetActiveScene(), false);
                    break;
            }
        }

        private void playSEAudio(AudioClip clip, float vol = 1.0f)
        {
            var source = gameObject.AddComponent<AudioSource>();
            source.clip = clip;
            source.volume = SEVolume;
            source.Play();
            ses.Add(source);
        }

        private System.Random rng = new System.Random();
        private int lastIdx = -1;
        private void playBGM(Scene scene, bool? flag = null)
        {
            if(scene.buildIndex == 2)
            {
                if (flag.HasValue)
                {
                    var clip = AssetBundleManager.Instance.LoadAsset<AudioClip>("audio", flag.Value ? "win" : "lose");
                    if (bgm.clip != clip && (bgm.clip == null || bgm.clip.name != clip.name))
                        bgm.clip = clip;
                }
                else
                {
                    var idx = rng.Next(1, 6);
                    while (idx == lastIdx)
                        idx = rng.Next(1, 6);
                    bgm.clip = AssetBundleManager.Instance.LoadAsset<AudioClip>("audio", "battle" + idx);
                    lastIdx = idx;
                }
                bgm.loop = false;
                bgm.Play();
            }
            else if(scene.buildIndex == 1)
            {
                lastIdx = -1;
                var clip = AssetBundleManager.Instance.LoadAsset<AudioClip>("audio", "lobby");
                if (bgm.clip != clip && (bgm.clip == null || bgm.clip.name != clip.name))
                {
                    bgm.clip = clip;
                    bgm.loop = true;
                    bgm.Play();
                }
            }
        }
    }
}
