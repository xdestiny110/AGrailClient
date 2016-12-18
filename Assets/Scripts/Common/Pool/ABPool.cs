using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Common
{
    public class ABPool:Singleton<ABPool>
    {
        Dictionary<string, List<IPoolUser>> instWaitingUsers = new Dictionary<string, List<IPoolUser>>();
        Dictionary<string, List<IPoolUser>> assetWaitingUsers = new Dictionary<string, List<IPoolUser>>();      

        public void InstanceAsync(string userBundleName, IPoolUser user)
        {
            InsertWaiter(instWaitingUsers, userBundleName, user);
            //测试代码 直接从resources中加载
            Object asset = LoadFromResource(userBundleName);
            OnAssetLoadDone(userBundleName, asset);
        }

        public void GiveBack(Object o)
        {
            if(o is GameObject)
            {
                GameObject.DestroyImmediate(o);
            }
        }

        public void OnAssetLoadDone(string userBundleName, object asset)
        {
            List<IPoolUser> usrList;
                        
            if (asset is GameObject && instWaitingUsers.TryGetValue(userBundleName, out usrList))
            {
                foreach (IPoolUser usr in usrList)
                {
                    usr.OnPoolInstanceDone(userBundleName, GameObject.Instantiate(asset as GameObject));
                }
                usrList.Clear();
            }
            
            if(assetWaitingUsers.TryGetValue(userBundleName, out usrList))
            {
                foreach (IPoolUser usr in usrList)
                {
                    usr.OnPoolInstanceDone(userBundleName, asset);
                }
                usrList.Clear();
            }
        }

        Object LoadFromResource(string userBundleName)
        {
            string loadableName = ABName.UserBundleName2LoadableName(userBundleName);
            ABPath path = ABName.Userbundlename2PathType(userBundleName);
            switch(path)
            {
                case ABPath.Ads:
                    break;
                case ABPath.Audio:
                    break;
                case ABPath.UIWin:
                    return Resources.Load(string.Format("UI/{0}", loadableName));
            }
            return null;
        }

        void InsertWaiter(Dictionary<string, List<IPoolUser>> waiterList, string userBundleName, IPoolUser usr)
        {
            if(!waiterList.ContainsKey(userBundleName))
            {
                waiterList.Add(userBundleName, new List<IPoolUser>());
            }
            if(!waiterList[userBundleName].Contains(usr))
            {
                waiterList[userBundleName].Add(usr);
            }
        }
    }
}