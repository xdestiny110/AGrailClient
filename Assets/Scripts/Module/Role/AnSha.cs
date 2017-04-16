using UnityEngine;
using System.Collections;
using System;

namespace AGrail
{
    public class AnSha : RoleBase
    {
        public override RoleID RoleID
        {
            get
            {
                return RoleID.AnSha;
            }
        }

        public override string RoleName
        {
            get
            {
                return "暗杀者";
            }
        }

        public override Card.CardProperty RoleProperty
        {
            get
            {
                return Card.CardProperty.技;
            }
        }
    }
}
