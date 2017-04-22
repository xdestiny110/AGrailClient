using UnityEngine;
using System.Collections;
using System;

namespace AGrail
{
    public class NvWuShen : RoleBase
    {
        public override RoleID RoleID
        {
            get
            {
                return RoleID.NvWuShen;
            }
        }

        public override string RoleName
        {
            get
            {
                return "女武神";
            }
        }

        public override Card.CardProperty RoleProperty
        {
            get
            {
                return Card.CardProperty.圣;
            }
        }

        public override string Knelt
        {
            get
            {
                return "YingLingXingTai";
            }
        }
    }
}
