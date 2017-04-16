using UnityEngine;
using System.Collections;
using System;

namespace AGrail
{
    public static class RoleFactory
    {
        public static RoleBase Create(string roleName)
        {
            var type = Type.GetType("AGrail." + roleName);
            if(type == null)
                Debug.LogWarningFormat("Cannot create role! Rolename = {0}", roleName);            
            return (type == null) ? new Blank() : Activator.CreateInstance(type) as RoleBase;
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
        spMoDao,
    }
}

