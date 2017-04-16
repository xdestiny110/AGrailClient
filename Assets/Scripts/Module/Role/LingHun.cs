using UnityEngine;
using System.Collections;
using System;

namespace AGrail
{
    public class LingHun : RoleBase
    {
        public override RoleID RoleID
        {
            get
            {
                return RoleID.LingHun;
            }
        }

        public override string RoleName
        {
            get
            {
                return "灵魂术士";
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

        public override bool HasBlue
        {
            get
            {
                return true;
            }
        }
    }
}
