using UnityEngine;
using System.Collections;
using System;

namespace AGrail
{
    public class MoQiang : RoleBase
    {
        public override RoleID RoleID
        {
            get
            {
                return RoleID.MoQiang;
            }
        }

        public override string RoleName
        {
            get
            {
                return "魔枪";
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


