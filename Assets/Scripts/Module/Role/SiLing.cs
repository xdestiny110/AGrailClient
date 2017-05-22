using UnityEngine;
using System.Collections;
using System;

namespace AGrail
{
    public class SiLing : RoleBase
    {
        public override RoleID RoleID
        {
            get
            {
                return RoleID.SiLing;
            }
        }

        public override string RoleName
        {
            get
            {
                return "死灵法师";
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
