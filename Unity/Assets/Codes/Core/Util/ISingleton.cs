using System;

namespace Core
{
    public interface ISingleton
    {
        void Register();
        void Initialize();
    }

    public abstract class Singleton<T> : ISingleton where T : Singleton<T>, new()
    {
        private bool isDisposed;
        private static T instance;

        public static T Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new T();
                }

                return instance;
            }
        }

        void ISingleton.Register()
        {
            if (instance != null)
            {
                throw new Exception($"singleton register twice! {typeof(T).Name}");
            }

            instance = (T)this;
        }

        public virtual void Initialize()
        {
        }

        public virtual void Destroy()
        {
            instance = null;
        }


    }

}