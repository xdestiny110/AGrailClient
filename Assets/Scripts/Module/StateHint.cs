using Framework.AssetBundle;
using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AGrail
{
    public static class StateHint
    {
        private static Dictionary<string, Dictionary<string, string>> hint;

        static StateHint()
        {
            var txt = AssetBundleManager.Instance.LoadAsset<TextAsset>("battle", "hint").text;
            hint = JsonConvert.DeserializeObject<Dictionary<string, Dictionary<string, string>>>(txt);
        }

        public static string GetHint(StateEnum state, int additional = 0)
        {
            if (hint[state.ToString()].ContainsKey(additional.ToString()))
                return hint[state.ToString()][additional.ToString()];
            else
                return hint[state.ToString()]["0"];
        }

        public static string GetHint(uint state, int additional = 0)
        {
            return hint[Skill.GetSkill(state).SkillName][additional.ToString()];
        }

        public static string GetHint(string keyname, int additional = 0)
        {
            return hint[keyname][additional.ToString()];
        }
    }
}
