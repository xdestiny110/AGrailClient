using UnityEngine;
using System.Collections;
using System;

namespace AGrail
{
    public class ZhongCai : RoleBase
    {
        public override RoleID RoleID
        {
            get
            {
                return RoleID.ZhongCai;
            }
        }

        public override string RoleName
        {
            get
            {
                return "仲裁者";
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
                return "ShenPan";
            }
        }
    }
}
