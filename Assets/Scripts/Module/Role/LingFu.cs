using UnityEngine;
using System.Collections;
using System;

namespace AGrail
{
    public class LingFu : RoleBase
    {
        public override RoleID RoleID
        {
            get
            {
                return RoleID.LingFu;
            }
        }

        public override string RoleName
        {
            get
            {
                return "灵符师";
            }
        }

        public override Card.CardProperty RoleProperty
        {
            get
            {
                return Card.CardProperty.幻;
            }
        }

        public override bool HasCoverd
        {
            get
            {
                return true;
            }
        }
    }
}
