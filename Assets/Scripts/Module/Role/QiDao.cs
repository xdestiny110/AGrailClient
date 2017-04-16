using UnityEngine;
using System.Collections;
using System;

namespace AGrail
{
    public class QiDao : RoleBase
    {
        public override RoleID RoleID
        {
            get
            {
                return RoleID.QiDao;
            }
        }

        public override string RoleName
        {
            get
            {
                return "祈祷师";
            }
        }

        public override Card.CardProperty RoleProperty
        {
            get
            {
                return Card.CardProperty.幻;
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
                return "QiDao";
            }
        }
    }
}
