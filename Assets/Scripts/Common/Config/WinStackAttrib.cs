using UnityEngine;
using System.Collections;
using System.Collections.Generic;
namespace Common.Config
{
    public class WinStackAttrib
    {
        public static List<WinStackAttrib> all = new List<WinStackAttrib>();
        public string SceneName;
        public int Identity;
        public string StartWinName;
        public Vector3 Position;
        public Quaternion Rotation;
        public Vector3 scale;
        public List<int> group;

        public WinStackAttrib(string sceneName, int Identity, string startWinName, Vector3 position, Quaternion rotation, Vector3 scale, List<int> group)
        {
            this.SceneName = sceneName;
            this.Identity = Identity;
            this.StartWinName = startWinName;
            this.Position = position;
            this.Rotation = rotation;
            this.scale = scale;
            this.group = group;
        }
        public static void Parse(string fileName)
        {
            /*
             * 从文件中读取内容逐行解析，每一行对应一个WinStackAttrib对象并添加到all中，这时直接模拟完成添加动作
             */
            all.Add(new WinStackAttrib(Common.Constance.SceneNameSquare, 1, "Login", Vector3.forward*15, Quaternion.identity, Vector3.one, new List<int>()));       

            all.Add(new WinStackAttrib(Common.Constance.SceneNameSquare, 2, "PosterSquare", new Vector3(-3.56f,-4.24f,18.66f), Quaternion.Euler(0,0,0), Vector3.one*0.45f, new List<int>()));
            all.Add(new WinStackAttrib(Common.Constance.SceneNameSquare, 3, "PosterSquare", new Vector3(15.24f, -0.3f,18.52f) , Quaternion.Euler(0, 21.28f, 0), Vector3.one*0.4f, new List<int>()));
            all.Add(new WinStackAttrib(Common.Constance.SceneNameSquare, 4, "PosterSquare", new Vector3(-15.28f, 1.85f, 18.51f) , Quaternion.Euler(0, -7.4f,0 ), Vector3.one*0.4f, new List<int>()));

            all.Add(new WinStackAttrib(Common.Constance.SceneNameLobby, 5, "PosterLobby", new Vector3(15.5f, 2f, 18.37f) , Quaternion.Euler(0, 21.8f, 0), new Vector3(0.9f,1,1), new List<int> { 5,6,7,8}));
            all.Add(new WinStackAttrib(Common.Constance.SceneNameLobby, 6, "PosterLobby", new Vector3(-16f, 0.09f, 28.84f), Quaternion.Euler(0, -43.7f, 0), new Vector3(1.0f, 1.0f,1), new List<int> { 5, 6,7,8 }));
            all.Add(new WinStackAttrib(Common.Constance.SceneNameLobby, 7, "PosterLobby", new Vector3(-26.1f, 0.09f, 13.46f), Quaternion.Euler(0, -55.826f, 0), new Vector3(1.0f, 1.0f, 1), new List<int> { 5, 6,7,8 }));
            all.Add(new WinStackAttrib(Common.Constance.SceneNameLobby, 8, "PosterLobby", new Vector3(34.04f, 0.87f, 5.59f), Quaternion.Euler(0, 51.387f, 0), new Vector3(1.0f, 1.0f, 1), new List<int> { 5, 6 ,7,8}));

            all.Add(new WinStackAttrib(Common.Constance.SceneNameLobby, 9, "Time", new Vector3(-0.51f, 8.0f, 25.1f), Quaternion.identity, Vector3.one, new List<int> {}));

        }
    }
}