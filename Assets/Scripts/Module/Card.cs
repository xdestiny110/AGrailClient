using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using Framework.AssetBundle;

namespace AGrail
{
    public class Card
    {
        public uint ID { get; private set; }
        public CardType Type { get; private set; }
        public CardElement Element { get; private set; }
        public CardProperty Property { get; private set; }
        public CardName Name { get; private set; }
        public string AssetPath { get; private set; }
        public string Description { get; private set; }
        public int SkillNum { get; private set; }
        public List<string> SkillNames = new List<string>();

        private static Dictionary<uint, Card> cardDict = new Dictionary<uint, Card>();
        static Card()
        {
            var txt = AssetBundleManager.Instance.LoadAsset<TextAsset>("battle", "cardDB").text;
            var strs = txt.Split('\n');
            foreach (var v in strs)
            {
                if (string.IsNullOrEmpty(v)) continue;
                var s = v.Trim(" \t\r\n".ToCharArray());
                var t = s.Split('\t');
                var c = new Card()
                {
                    ID = uint.Parse(t[0]),
                    Type = (CardType)Enum.Parse(typeof(CardType), t[1], true),
                    Element = (CardElement)Enum.Parse(typeof(CardElement), t[2], true),
                    Property = (CardProperty)Enum.Parse(typeof(CardProperty), t[3], true),
                    Name = (CardName)Enum.Parse(typeof(CardName), t[4], true),
                    AssetPath = t[5],
                    Description = t[6],
                    SkillNum = int.Parse(t[7])
                };
                for(int i = 0; i < c.SkillNum; i++)
                    c.SkillNames.Add(t[8 + i]);
                cardDict.Add(c.ID, c);
            }
        }

        public static Card GetCard(uint cardID)
        {
            if (cardDict.ContainsKey(cardID))
                return cardDict[cardID];
            else
            {
                Debug.LogErrorFormat("Can not find card id {0}", cardID);
                return null;
            }
        }

        private Card() { }

        public bool HasSkill(string skillName)
        {
            foreach(var v in SkillNames)
            {
                if (v == skillName)
                    return true;
            }
            return false;
        }

        public bool HasSkill(uint skillID)
        {
            return HasSkill(Skill.GetSkill(skillID).SkillName);
        }

        public enum CardType
        {
            magic,
            attack,
            unique
        }

        public enum CardElement
        {
            none,
            earth,
            wind,
            light,
            fire,
            thunder,
            water,
            darkness,
        }

        public enum CardProperty
        {
            无, 幻, 圣, 咏, 技, 血
        }

        public enum CardName
        {
            圣盾,
            虚弱,
            中毒,
            魔弹,
            地裂斩,
            风神斩,
            火焰斩,
            水涟斩,
            雷光斩,
            圣光,
            暗灭,
            五行束缚,
            同生共死,
            挑衅,
            灵魂链接,
            永恒乐章
        }
    }
}
