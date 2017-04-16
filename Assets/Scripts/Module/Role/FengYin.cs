using UnityEngine;
using System.Collections;
using System;

namespace AGrail
{
    public class FengYin : RoleBase
    {
        public override RoleID RoleID
        {
            get
            {
                return RoleID.FengYin;
            }
        }

        public override string RoleName
        {
            get
            {
                return "封印师";
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


