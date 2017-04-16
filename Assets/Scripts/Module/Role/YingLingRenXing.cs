using UnityEngine;
using System.Collections;
using System;

namespace AGrail
{
    public class YingLingRenXing : RoleBase
    {
        public override RoleID RoleID
        {
            get
            {
                return RoleID.YingLingRenXing;
            }
        }

        public override string RoleName
        {
            get
            {
                return "英灵人形";
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
