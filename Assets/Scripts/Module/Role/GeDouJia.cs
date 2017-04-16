using UnityEngine;
using System.Collections;
using System;

namespace AGrail
{
    public class GeDouJia : RoleBase
    {
        public override RoleID RoleID
        {
            get
            {
                return RoleID.GeDouJia;
            }
        }

        public override string RoleName
        {
            get
            {
                return "格斗家";
            }
        }

        public override Card.CardProperty RoleProperty
        {
            get
            {
                return Card.CardProperty.技;
            }
        }

        public override bool HasYellow
        {
            get
            {
                return true;
            }
        }
    }
}
