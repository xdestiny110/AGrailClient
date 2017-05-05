using UnityEngine;
using System.Collections;

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
        public Skill(uint skillID, string skillName, SkillType type, SkillProperty property, SkillCost cost, string desc)
        {
            SkillID = skillID;
            SkillName = skillName;
            SkillType = type;
            Property = property;
            Cost = cost;
            Description = desc;
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
        Cristal,

    }
}


