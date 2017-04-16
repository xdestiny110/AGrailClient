using UnityEngine;
using System.Collections;
using System;

namespace AGrail
{
    public class MoGong : RoleBase
    {
        public override RoleID RoleID
        {
            get
            {
                return RoleID.MoGong;
            }
        }

        public override string RoleName
        {
            get
            {
                return "魔弓";
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


