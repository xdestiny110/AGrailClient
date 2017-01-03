using System.Collections.Generic;
using UnityEngine;

namespace Framework.UI
{
    public abstract class CustomUIComponent : MonoBehaviour
    {
        [SerializeField]
        private string uiName = string.Empty; 
        public string UIName { get { return uiName; } }

        public List<UnityEngine.EventSystems.UIBehaviour> uiLists = new List<UnityEngine.EventSystems.UIBehaviour>();
    }
}
