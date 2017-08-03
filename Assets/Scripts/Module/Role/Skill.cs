using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using Framework.AssetBundle;

namespace AGrail
{
    public class Skill
    {
        public uint SkillID;
        public string SkillName;
        public SkillType SkillType;
        public SkillProperty Property;
        public SkillCost Cost;
        public string Description;
        private Skill(uint skillID, string skillName, SkillType type, SkillProperty property, SkillCost cost, string desc)
        {
            SkillID = skillID;
            SkillName = skillName;
            SkillType = type;
            Property = property;
            Cost = cost;
            Description = desc;
        }

        private static Dictionary<uint, Skill> skillDict = new Dictionary<uint, Skill>();
        static Skill()
        {
            var txt = AssetBundleManager.Instance.LoadAsset<TextAsset>("battle", "skillDB").text;
            var strs = txt.Split('\n');
            foreach (var v in strs)
            {
                if (string.IsNullOrEmpty(v)) continue;
                var s = v.Trim(" \t\r\n".ToCharArray());
                var t = s.Split('\t');
                var skillID = uint.Parse(t[0]);
                var skillName = t[1];
                var skillType = (SkillType)Enum.Parse(typeof(SkillType), t[2]);
                var skillProperty = (SkillProperty)Enum.Parse(typeof(SkillProperty), t[3]);
                var skillCost = (SkillCost)Enum.Parse(typeof(SkillCost), t[4]);
                var skillDesc = t[5];
                skillDict.Add(skillID, new Skill(skillID, skillName, skillType, skillProperty, skillCost, skillDesc));
            }
        }

        public static Skill GetSkill(uint skillID)
        {
            if (skillDict.ContainsKey(skillID))
                return skillDict[skillID];
            return null;
        }
    }

    public enum SkillType
    {
        法术,
        被动,
        启动,
        响应
    }

    public enum SkillProperty
    {
        普通,
        独有技,
        必杀技
    }

    public enum SkillCost
    {
        None,
        Gem,
        Crystal,
    }
}


