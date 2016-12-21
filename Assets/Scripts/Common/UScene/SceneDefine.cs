using UnityEngine;
using System.Collections;
namespace UScene
{
    public static class SceneDefine
    {
        public enum SceneType
        {
            NULL = 0,
            Loading,
            Login,
            Lobby,
            Room,
        }

        public const string LoginPre = "Login";
        public const string LobbyPre = "Lobby";
        public const string RoomPre = "Room";
        public const string LoadingName = "Loading";
        
        public static SceneType CurrentSceneType()
        {
            string name = UnityEngine.SceneManagement.SceneManager.GetActiveScene().name;
            return SceneNameToType(name);
        }

        public static SceneType SceneNameToType(string name)
        {
            if (name.StartsWith(LoginPre))
                return SceneType.Login;
            if (name.StartsWith(LobbyPre))
                return SceneType.Lobby;
            if (name.StartsWith(RoomPre))
                return SceneType.Room;
            if (name.StartsWith(LoadingName))
                return SceneType.Loading;
            return SceneType.NULL;
        }
    }
}