using UnityEngine;
using System.Collections;
using System;

namespace AGrail
{
    public class YuanSu : RoleBase
    {
        public override RoleID RoleID
        {
            get
            {
                return RoleID.YuanSu;
            }
        }

        public override string RoleName
        {
            get
            {
                return "元素师";
            }
        }

        public override Card.CardProperty RoleProperty
        {
            get
            {
                return Card.CardProperty.咏;
            }
        }

        public override bool HasYellow
        {
            get
            {
                return true;
            }
        }
    }
}
