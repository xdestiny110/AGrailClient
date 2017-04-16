using UnityEngine;
using System.Collections;
using System;

namespace AGrail
{
    public class MoNv : RoleBase
    {
        public override RoleID RoleID
        {
            get
            {
                return RoleID.MoNv;
            }
        }

        public override string RoleName
        {
            get
            {
                return "苍炎魔女";
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


