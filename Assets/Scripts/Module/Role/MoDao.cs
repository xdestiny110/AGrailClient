using UnityEngine;
using System.Collections;
using System;

namespace AGrail
{
    public class MoDao : RoleBase
    {
        public override RoleID RoleID
        {
            get
            {
                return RoleID.MoDao;
            }
        }

        public override string RoleName
        {
            get
            {
                return "魔导师";
            }
        }

        public override Card.CardProperty RoleProperty
        {
            get
            {
                return Card.CardProperty.咏;
            }
        }
    }
}
