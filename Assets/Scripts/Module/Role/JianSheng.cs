using UnityEngine;
using System.Collections;
using System;

namespace AGrail
{
    public class JianSheng : RoleBase
    {
        public override RoleID RoleID
        {
            get
            {
                return RoleID.JianSheng;
            }
        }

        public override string RoleName
        {
            get
            {
                return "风之剑圣";
            }
        }

        public override Card.CardProperty RoleProperty
        {
            get
            {
                return Card.CardProperty.技;
            }
        }
    }
}


