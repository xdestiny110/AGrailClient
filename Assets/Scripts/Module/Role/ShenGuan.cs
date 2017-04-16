using UnityEngine;
using System.Collections;
using System;

namespace AGrail
{
    public class ShenGuan : RoleBase
    {
        public override RoleID RoleID
        {
            get
            {
                return RoleID.ShenGuan;
            }
        }

        public override string RoleName
        {
            get
            {
                return "神官";
            }
        }

        public override Card.CardProperty RoleProperty
        {
            get
            {
                return Card.CardProperty.圣;
            }
        }
    }
}
