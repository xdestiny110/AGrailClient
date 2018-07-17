using UnityEngine;
using UnityEngine.UI;
using Framework.UI;
using Framework.Message;
using System;

namespace AGrail
{
    public class InputBox : UIBase
    {
        [SerializeField]
        private InputField inpt;
        [SerializeField]
        private Text message;
        [SerializeField]
        private Button btnOK;
        [SerializeField]
        private Button btnCancel;

        public override string Type
        {
            get
            {
                return WindowType.InputBox.ToString();
            }
        }

        public override object[] Parameters
        {
            get
            {
                return base.Parameters;
            }

            set
            {
                base.Parameters = value;
                btnOK.onClick.AddListener(() => { ((Action<string>)value[0])(inpt.text); });
                btnCancel.onClick.AddListener(() => { ((Action<string>)value[1])(inpt.text); });
                message.text = (string)value[2];
            }
        }
    }
}


