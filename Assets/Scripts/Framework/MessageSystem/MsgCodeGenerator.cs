namespace Framework.Message
{
#if UNITY_EDITOR
    using UnityEngine;
    using UnityEditor;
    using System.Collections.Generic;

    [InitializeOnLoad]
    public class MsgCodeGenerator : ScriptableObject
    {
        public const string configPath = "Assets/Editor/MessageCodeGenerator/Config.asset";
        public const string messageTypePath = "Assets/Scripts/Framework/MessageSystem/MessageType.cs";

        private static MsgCodeGenerator instance;
        public static MsgCodeGenerator Instance
        {
            get
            {
                if(instance == null)
                {
                    instance = AssetDatabase.LoadAssetAtPath<MsgCodeGenerator>(configPath);
                    if(instance == null)
                    {
                        instance = CreateInstance<MsgCodeGenerator>();
                        AssetDatabase.CreateAsset(instance, configPath);
                        AssetDatabase.Refresh();
                    }
                }
                return instance;
            }
        }

        [SerializeField]
        [HideInInspector]
        private List<string> msgtypes = new List<string>();

    }
#endif
}


