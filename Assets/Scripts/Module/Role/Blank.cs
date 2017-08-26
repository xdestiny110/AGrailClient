using UnityEngine;
using System.Collections;
using System;

namespace AGrail
{
    public class Blank : RoleBase
    {
        public override RoleID RoleID
        {
            get
            {
                return RoleID.Blank;
            }
        }

        public override string RoleName
        {
            get
            {
                return "白板";
            }
        }

        public override Card.CardProperty RoleProperty
        {
            get
            {
                return Card.CardProperty.技;
            }
        }

        public override string HeroName
        {
            get
            {
                return "白板";
            }
        }

        public override uint Star
        {
            get
            {
                return 30;
            }
        }
    }
}
