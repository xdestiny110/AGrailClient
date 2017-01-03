namespace Framework
{
    namespace UI
    {
        public enum WindowMsg
        {
            Null = 0,
            Show,
            Uninteractable,
            Hide,
            Destroy
        }

        public enum WindowType
        {
            Null = 0,
            MessageBox,
            Login,
        }

        public static class WindowTypeExtension
        {
            public static WindowType GetWindowType(this Window window)
            {
                //if(typeof(window) == typeof())
                return WindowType.Null;
            }
        }

    }
}

