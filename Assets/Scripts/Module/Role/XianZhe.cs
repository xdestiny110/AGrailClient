using UnityEngine;
using System.Collections;
using System;

namespace AGrail
{
    public class XianZhe : RoleBase
    {
        public override RoleID RoleID
        {
            get
            {
                return RoleID.XianZhe;
            }
        }

        public override string RoleName
        {
            get
            {
                return "贤者";
            }
        }

        public override Card.CardProperty RoleProperty
        {
            get
            {
                return Card.CardProperty.咏;
            }
        }
    }
}
