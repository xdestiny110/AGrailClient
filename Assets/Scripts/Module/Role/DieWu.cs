using UnityEngine;
using System.Collections;
using System;

namespace AGrail
{
    public class DieWu : RoleBase
    {
        public override RoleID RoleID
        {
            get
            {
                return RoleID.DieWu;
            }
        }

        public override string RoleName
        {
            get
            {
                return "蝶舞者";
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

        public override bool HasCoverd
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
                return "DiaoLing";
            }
        }
    }
}
