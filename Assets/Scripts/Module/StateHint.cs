using Framework.AssetBundle;
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
			LitJson.JsonData data = LitJson.JsonMapper.ToObject(txt);
			foreach (var v in data.Keys) {
				var d = new Dictionary<string, string> ();
				hint.Add (v, d);
				foreach (var u in data[v].Keys)
					d.Add (u, data [v] [u].ToString());
			}            
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
