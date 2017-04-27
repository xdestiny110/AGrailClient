using UnityEngine;
using UnityEngine.UI;
using System.Collections;

namespace AGrail
{
    public class DialogUI : MonoBehaviour
    {
        [SerializeField]
        private InputField inpt;

        public void OnDialogInputSubmit(string str)
        {
            Dialog.Instance.SendTalk(inpt.text);
            inpt.text = string.Empty;
        }

    }
}

