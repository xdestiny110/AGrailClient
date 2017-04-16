using UnityEngine;
using System.Collections;
using System;

namespace AGrail
{
    public class TianShi : RoleBase
    {
        public override RoleID RoleID
        {
            get
            {
                return RoleID.TianShi;
            }
        }

        public override string RoleName
        {
            get
            {
                return "天使";
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
