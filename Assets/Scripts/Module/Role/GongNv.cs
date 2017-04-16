using UnityEngine;
using System.Collections;
using System;

namespace AGrail
{
    public class GongNv : RoleBase
    {
        public override RoleID RoleID
        {
            get
            {
                return RoleID.GongNv;
            }
        }

        public override string RoleName
        {
            get
            {
                return "弓之女神";
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


