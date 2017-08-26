using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using Framework.AssetBundle;

namespace AGrail
{
    public class Mation
    {
        public uint MationID;
        public string MationName;
        public string MationTag;
        public string MationDesc;
        private Mation(uint ID, string Name, string Tag, string Desc)
        {
            MationID = ID;
            MationName = Name;
            MationTag = Tag;
            MationDesc = Desc;
        }

        private static Dictionary<uint, Mation> MationDict = new Dictionary<uint, Mation>();
        static Mation()
        {
            var txt = AssetBundleManager.Instance.LoadAsset<TextAsset>("battle", "MationDB").text;
            var strs = txt.Split('\n');
            foreach (var v in strs)
            {
                if (string.IsNullOrEmpty(v)) continue;
                var s = v.Trim(" \t\r\n".ToCharArray());
                var t = s.Split('\t');
                var ID = uint.Parse(t[0]);
                var Name = t[1];
                var Tag = t[2];
                var Desc = "";
                for (uint i = 3; i < t.Length; i++)
                    if(!string.IsNullOrEmpty(t[i]))
                    Desc += t[i] + "\n";

                MationDict.Add(ID, new Mation(ID, Name, Tag, Desc));
            }
        }

        public static Dictionary<uint,Mation> GetMation(uint RID)
        {
            Dictionary<uint, Mation> MationDic = new Dictionary<uint, Mation>();
            for (uint i = 0; i < 10; i++)
                if (MationDict.ContainsKey(RID * 100 + i))
                    MationDic.Add(RID * 100 + i, MationDict[RID * 100 + i]);
            return MationDic;
        }
    }
}
