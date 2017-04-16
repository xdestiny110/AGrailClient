using UnityEngine;
using System.Collections;
using System;

namespace AGrail
{
    public class YongZhe : RoleBase
    {
        public override RoleID RoleID
        {
            get
            {
                return RoleID.YongZhe;
            }
        }

        public override string RoleName
        {
            get
            {
                return "勇者";
            }
        }

        public override Card.CardProperty RoleProperty
        {
            get
            {
                return Card.CardProperty.血;
            }
        }

        public override bool HasYellow
        {
            get
            {
                return true;
            }
        }

        public override string Knelt
        {
            get
            {
                return "JingPiLiJie";
            }
        }
    }
}
