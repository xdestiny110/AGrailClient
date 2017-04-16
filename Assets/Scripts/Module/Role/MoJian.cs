using UnityEngine;
using System.Collections;
using System;

namespace AGrail
{
    public class MoJian : RoleBase
    {
        public override RoleID RoleID
        {
            get
            {
                return RoleID.MoJian;
            }
        }

        public override string RoleName
        {
            get
            {
                return "魔剑";
            }
        }

        public override Card.CardProperty RoleProperty
        {
            get
            {
                return Card.CardProperty.幻;
            }
        }
    }
}
