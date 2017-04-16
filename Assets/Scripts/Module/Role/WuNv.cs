using UnityEngine;
using System.Collections;
using System;

namespace AGrail
{
    public class WuNv : RoleBase
    {
        public override RoleID RoleID
        {
            get
            {
                return RoleID.WuNv;
            }
        }

        public override string RoleName
        {
            get
            {
                return "巫女";
            }
        }

        public override Card.CardProperty RoleProperty
        {
            get
            {
                return Card.CardProperty.血;
            }
        }
    }
}
