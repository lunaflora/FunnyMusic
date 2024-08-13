namespace Framework
{
    public interface ITimer
    {
        void Handle(object args);

    }

    public abstract class ATimer<T> : ITimer where T : class
    {

        public void Handle(object args)
        {
            Excute(args as T);
        }

        public abstract void Excute(T a);
        
    
    }
}