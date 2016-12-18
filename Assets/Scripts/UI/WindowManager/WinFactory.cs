using UnityEngine;
using System.Collections;
namespace UI
{
    //should auto generate
    public class WinFactory
    {
        public static Window Create(string windowName, int identity)
        {
            switch (windowName)
            {
                //case "Login":
                //    return new Login(identity);
                //case "PosterSquare":
                //    return new PosterSquare(identity);
                //case "PosterLobby":
                //    return new PosterLobby(identity);
                //case "Time":
                //    return new UI.Time(identity);
                /*
                 * compisewindow 使用_p _c结尾来表示是你或是子
                 * case "compise_p":
                 * return new CompiseWindow(identity);
                 */
                default:
                    return null;
            }
           
        }
       
    }
}