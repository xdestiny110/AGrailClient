namespace Framework
{
    public class Singleton<T> where T : new()
    {
        protected static T instance = default(T);
        protected static object locker = new object();
        public static T Instance
        {
            get
            {
                if(instance == null)
                {
                    lock (locker)
                    {
                        if (instance == null)
                            instance = new T();
                    }
                }
                return instance;
            }
        }

        protected Singleton(){}
    }
}


