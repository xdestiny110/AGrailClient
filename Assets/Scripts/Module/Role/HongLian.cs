using UnityEngine;
using System.Collections;
using System;

namespace AGrail
{
    public class HongLian : RoleBase
    {
        public override RoleID RoleID
        {
            get
            {
                return RoleID.HongLian;
            }
        }

        public override string RoleName
        {
            get
            {
                return "红莲骑士";
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
                return "ReXueFeiTeng";
            }
        }
    }
}


