using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace AGrail
{
    [System.Serializable]
    public enum Team
    {
        Blue = 0,
        Red,
        Other,
    }

    public static class Util
    {
        public static Team GetOtherTeam(Team team)
        {
            return (team == Team.Blue) ? Team.Red : Team.Blue;
        }

        public static bool HasCard(uint skillID, List<uint> hands, uint num = 1)
        {
            uint count = 0;
            foreach (var v in hands)
            {
                if (Card.GetCard(v).HasSkill(skillID)) count++;
            }
            return count >= num;
        }
        public static bool HasCard(Card.CardElement cardElement, List<uint> hands, uint num = 1)
        {
            uint count = 0;
            foreach (var v in hands)
            {
                if ((Card.GetCard(v).Element) == cardElement) count++;
            }
            return count >= num;

        }
        public static bool HasCard(Card.CardType cardType, List<uint> hands, uint num = 1)
        {
            uint count = 0;
            foreach (var v in hands)
            {
                if ((Card.GetCard(v).Type) == cardType) count++;
            }
            return count >= num;
        }
        public static bool HasCard(string condition, List<uint> hands, uint num = 2)
        {
            var elements = new uint[7];
            hands.ForEach(v =>
            {
                uint e = (uint)Card.GetCard(v).Element;
                if (e > 0)
                    elements[e - 1]++;
            });
            uint count = 0;
            switch (condition)
            {
                case "same":
                    count = elements.Max();
                    break;
                case "differ":
                    count = (uint)elements.Count(v => { return v > 0; });
                    break;
                default:
                    break;
            }
            return count >= num;
        }
    }
}
