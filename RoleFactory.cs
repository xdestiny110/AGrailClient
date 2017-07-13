using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;

namespace AGrail
{
    public static class RoleFactory
    {
        private static Dictionary<string, RoleBase> pool = new Dictionary<string, RoleBase>();

        public static RoleBase Create(string roleName)
        {
            if (pool.ContainsKey(roleName))
                return pool[roleName];
            var type = Type.GetType("AGrail." + roleName);
            if(type == null)
                Debug.LogWarningFormat("Cannot create role! Rolename = {0}", roleName);            
            var r = (type == null) ? new Blank() : Activator.CreateInstance(type) as RoleBase;
            pool.Add(roleName, r);
            return r;
        }

        public static RoleBase Create(RoleID roleID)
        {
            return Create(roleID.ToString());
        }

        public static RoleBase Create(uint roleID)
        {
            return Create((RoleID)roleID);
        }
    }

    public enum RoleID
    {
        Blank = 0,
        JianSheng,
        KuangZhan,
        GongNv,
        FengYin,
        AnSha,
        ShengNv,
        TianShi,
        MoDao,
        MoJian,
        ShengQiang,
        YuanSu,
        MaoXian,
        SiLing,
        ZhongCai,
        ShenGuan,
        QiDao,
        XianZhe,
        LingFu,
        JianDi,
        GeDouJia,
        YongZhe,
        LingHun,
        WuNv,
        DieWu,
        NvWuShen,
        MoGong,
        YingLingRenXing,
        HongLian,
        MoQiang,
        MoNv,
        ShiRen,
        spMoDao=108,
    }
}

