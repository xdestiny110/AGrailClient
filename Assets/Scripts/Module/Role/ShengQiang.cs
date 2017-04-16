using UnityEngine;
using System.Collections;
using System;

namespace AGrail
{
    public class ShengQiang : RoleBase
    {
        public override RoleID RoleID
        {
            get
            {
                return RoleID.ShengQiang;
            }
        }

        public override string RoleName
        {
            get
            {
                return "圣枪";
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
