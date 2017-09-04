using Framework.AssetBundle;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace AGrail
{
    public class PlayerIco : MonoBehaviour
    {
        [SerializeField]
        private Image Team;
        [SerializeField]
        private Image Hero;
        [SerializeField]
        private GameObject canSelect;
        [SerializeField]
        private GameObject selected;
        [SerializeField]
        public Button MVP;

        public uint IcoID
        {
            set
            {
                Hero.sprite = AssetBundleManager.Instance.LoadAsset<Sprite>("hero_s", BattleData.Instance.GetPlayerInfo(value).role_id.ToString() + "S");
                Team.sprite = AssetBundleManager.Instance.LoadAsset<Sprite>("battle_texture", "b02_chara_"+ (BattleData.Instance.GetPlayerInfo(value).team == 1 ?"red":"blue") );
            }
        }
        public uint heroid = 0;
        public uint HeroID
        {
            get
            {
                return heroid;
            }
            set
            {
                Hero.sprite = Hero.sprite = AssetBundleManager.Instance.LoadAsset<Sprite>("hero_h", value.ToString() + "h");
                heroid = value;
            }
            
        }
        public uint ID { get; set; }


        public bool canselect
        {
            set
            {
                canSelect.SetActive(value);
            }
        }
        
        public bool Selected
        {
            set
            {
                selected.SetActive(value);
            }
            get
            {
                return selected.activeSelf;
            }
        }
    }
}
