namespace Framework
{
    public interface IDisposable
    {
        void Dispose();
    }

    public interface ISupportInitialize
    {
        void BeginInit();
        void EndInit();
    }
    
    /// <summary>
    /// 用于需要初始化的静态数据,比如配置管理数据
    /// </summary>
    public abstract class StaticObject: ISupportInitialize
    {
        public virtual void BeginInit()
        {
        }

        public virtual void EndInit()
        {
        }

    }
    
    public abstract class DynamicObject: ISupportInitialize,IDisposable
    {
        public virtual void BeginInit()
        {
        }

        public virtual void EndInit()
        {
        }

        public virtual void Dispose()
        {
            
        }

    }
}