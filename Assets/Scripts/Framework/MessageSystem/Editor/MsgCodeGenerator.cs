namespace Framework.Message
{
    using UnityEngine;
    using UnityEditor;
    using System.Collections.Generic;
    using System;
    using network;

    [InitializeOnLoad]
    public class MsgCodeGenerator : ScriptableObject
    {
        public const string ConfigPath = "Assets/Scripts/Framework/MessageSystem/Editor/Config.asset";
        public const string MessageTypePath = "Assets/Scripts/Framework/MessageSystem/MessageType.cs";

        private static MsgCodeGenerator instance;
        public static MsgCodeGenerator Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = AssetDatabase.LoadAssetAtPath<MsgCodeGenerator>(ConfigPath);
                    if (instance == null)
                    {
                        instance = CreateInstance<MsgCodeGenerator>();
                        AssetDatabase.CreateAsset(instance, ConfigPath);
                        AssetDatabase.Refresh();
                    }
                }
                return instance;
            }
        }

        [HideInInspector]
        public List<string> msgTypesConst = new List<string>()
        {
            "Null = 0",
            "OnConnect",
            "OnDisconnect",
            "OnReconnect",
            "OnUICreate",
            "OnUIDestroy",
            "OnUIShow",
            "OnUIHide",
            "OnUIPause",
            "OnUIResume",
        };

        [HideInInspector]
        public List<string> msgTypesProto = new List<string>(Enum.GetNames(typeof(ProtoNameIds)));


        [HideInInspector]
        public List<string> msgTypes = new List<string>();

    }
}


