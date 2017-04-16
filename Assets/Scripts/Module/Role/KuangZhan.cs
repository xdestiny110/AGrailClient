using UnityEngine;
using System.Collections;
using System;

namespace AGrail
{
    public class KuangZhan : RoleBase
    {
        public override RoleID RoleID
        {
            get
            {
                return RoleID.KuangZhan;
            }
        }

        public override string RoleName
        {
            get
            {
                return "狂战士";
            }
        }

        public override Card.CardProperty RoleProperty
        {
            get
            {
                return Card.CardProperty.血;
            }
        }
    }
}


