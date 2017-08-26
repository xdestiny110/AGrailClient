using UnityEngine;
using System.Collections;
using Framework.UI;
using System;
using UnityEngine.UI;
using Framework.Message;
using System.Collections.Generic;

namespace AGrail
{
    public class ChooseArgsUI : WindowsBase
    {
        [SerializeField]
        private Dropdown args;
        [SerializeField]
        private Text title;

        private List<List<uint>> argsCache = new List<List<uint>>();

        public override WindowType Type
        {
            get
            {
                return WindowType.ChooseArgsUI;
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
                var t = value[0].ToString();
                argsCache = (List<List<uint>>)value[1];
                if (t == "Skill")
                {
                    title.text = "选择技能";
                    for (int i = 0; i < argsCache.Count; i++)
                        args.options.Add(new Dropdown.OptionData() { text = Skill.GetSkill(argsCache[i][0]).SkillName });
                }
                else if(t == "Card")
                {
                    title.text = "选择卡牌";
                    for (int i = 0; i < argsCache.Count; i++)
                        args.options.Add(new Dropdown.OptionData() { text = Card.GetCard(argsCache[i][0]).Name.ToString() });
                }
                else if(t == "Energy")
                {
                    title.text = "选择能量";
                    for (int i = 0; i < argsCache.Count; i++)
                        args.options.Add(new Dropdown.OptionData()
                        {
                            text = string.Format("{0}个宝石与{1}个水晶", argsCache[i][0], argsCache[i][1])
                        });
                }
                else if(t == "Heal")
                {
                    title.text = "选择治疗数";
                    for (int i = 0; i < argsCache.Count; i++)
                        args.options.Add(new Dropdown.OptionData()
                        {
                            text = string.Format("{0}个治疗", argsCache[i][0])
                        });
                }
                else
                {
                    //使用传入的参数构造选择列表
                    title.text = t;
                    var m = value[2] as List<string>;
                    for (int i = 0; i < argsCache.Count; i++)
                        args.options.Add(new Dropdown.OptionData()
                        {
                            text = string.Format("{0}{1}", argsCache[i][0], m[i])
                        });
                }
                args.RefreshShownValue();
                BattleData.Instance.Agent.SelectArgs.Clear();
                BattleData.Instance.Agent.SelectArgs.AddRange(argsCache[0]);
            }
        }

        public void OnValueChanged(int idx)
        {
            BattleData.Instance.Agent.SelectArgs.Clear();
            BattleData.Instance.Agent.SelectArgs.AddRange(argsCache[idx]);
        }
    }
}

