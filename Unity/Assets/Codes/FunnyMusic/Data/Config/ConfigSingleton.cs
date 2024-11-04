using System;
using Core;

namespace FunnyMusic
{
    public abstract class ConfigSingleton<T> :  ISingleton where T : ConfigSingleton<T>, new()
    { 
        private static T instance;

        public static T Instance
        {
            get { return instance; }
        }

        void ISingleton.Register()
        {
            if (instance != null)
            {
                throw new Exception($"singleton register twice! {typeof(T).Name}");
            }

            instance = (T)this;
        }

        void ISingleton.Destroy()
        {
            T t = instance;
            instance = null;
            t.Dispose();
        }

        public virtual void Initialize()
        {
            
        }
    
        public virtual void Dispose()
        {
        }
    }
}