using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

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

        private static List<string[]> cardStr = new List<string[]>();
        static Card()
        {
            var txt = (Resources.Load<TextAsset>("cardDB")).text;
            var strs = txt.Split('\n');
            foreach (var v in strs)
                cardStr.Add(v.Split('\t'));
        }

        public Card(uint cardID)
        {
            var t = cardStr[(int)cardID];
            ID = cardID;
            Type = (CardType)Enum.Parse(typeof(CardType), t[1], true);
            Element = (CardElement)Enum.Parse(typeof(CardElement), t[2], true);
            Property = (CardProperty)Enum.Parse(typeof(CardProperty), t[3], true);
            Name = (CardName)Enum.Parse(typeof(CardName), t[4], true);
            AssetPath = t[5];
        }

        public enum CardType
        {
            magic,
            attack
        }

        public enum CardElement
        {
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
            幻, 圣, 咏, 技, 血
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
        }




    }



}


