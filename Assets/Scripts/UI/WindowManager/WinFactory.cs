using UnityEngine;
using System.Collections;
namespace UI
{
    public class WinFactory
    {
        public const string ARRANGE = "Arrange";
        public const string LOGIN = "Login";
        public const string PAY = "Pay";
        public const string POSTERLOBBY = "PosterLobby";
        public const string POSTERSQUARE = "PosterSquare";
        public const string TEST = "Test";
        public const string TIME = "Time";
        public static Window Create(string windowName, int identity)
        {
            switch (windowName)
            {
                //case ARRANGE:
                //    return new Arrange(identity);
                //case LOGIN:
                //    return new Login(identity);
                //case PAY:
                //    return new Pay(identity);
                //case POSTERLOBBY:
                //    return new PosterLobby(identity);
                //case POSTERSQUARE:
                //    return new PosterSquare(identity);
                //case TEST:
                //    return new Test(identity);
                //case TIME:
                //    return new Time(identity);
                ///*
                // * compisewindow 使用_p _c结尾来表示是父或是子
                // * case "compise_p":
                // * return new CompiseWindow(identity);
                // */
                //default:
                //    return null;
            }
            return null;
        }
    }
}
