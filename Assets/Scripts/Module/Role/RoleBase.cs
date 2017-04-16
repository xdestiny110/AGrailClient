using UnityEngine;
using System.Collections;

namespace AGrail
{
    public abstract class RoleBase
    {
        public abstract RoleID RoleID { get; }
        public abstract string RoleName { get; }
        public abstract Card.CardProperty RoleProperty { get; }
        public virtual uint MaxHealCount { get { return 2; } }
        public virtual bool HasYellow { get { return false; } }
        public virtual bool HasBlue { get { return false; } }
        public virtual bool HasCoverd { get { return false; } }
        public virtual string Knelt { get { return null; } }
    }
}
