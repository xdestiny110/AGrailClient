using UnityEngine;
using System.Collections;
using System;

namespace AGrail
{
    public class ShengNv : RoleBase
    {
        public override RoleID RoleID
        {
            get
            {
                return RoleID.ShengNv;
            }
        }

        public override string RoleName
        {
            get
            {
                return "圣女";
            }
        }

        public override Card.CardProperty RoleProperty
        {
            get
            {
                return Card.CardProperty.圣;
            }
        }

        public override string Knelt
        {
            get
            {
                return "LianMin";
            }
        }
    }
}
