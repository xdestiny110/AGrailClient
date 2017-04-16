using UnityEngine;
using System.Collections;
using System;

namespace AGrail
{
    public class MaoXian : RoleBase
    {
        public override RoleID RoleID
        {
            get
            {
                return RoleID.MaoXian;
            }
        }

        public override string RoleName
        {
            get
            {
                return "冒险家";
            }
        }

        public override Card.CardProperty RoleProperty
        {
            get
            {
                return Card.CardProperty.幻;
            }
        }

        public override bool HasYellow
        {
            get
            {
                return true;
            }
        }
    }
}


