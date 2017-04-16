using UnityEngine;
using System.Collections;
using System;

namespace AGrail
{
    public class JianDi : RoleBase
    {
        public override RoleID RoleID
        {
            get
            {
                return RoleID.JianDi;
            }
        }

        public override string RoleName
        {
            get
            {
                return "剑帝";
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
