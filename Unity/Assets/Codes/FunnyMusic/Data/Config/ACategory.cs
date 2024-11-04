using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using Framework;


namespace FunnyMusic
{
    public abstract class ACategory : StaticObject
    {
        public abstract Type ConfigType { get; }
        
        public abstract string Path { set; get; }
        public abstract IConfig GetOne();
        public abstract IConfig[] GetAll();
        public abstract IConfig TryGet(int id);
        
        public abstract  string ConfigText
        {
            get;
            set;
        }

    }

    /// <summary>
    /// 管理该所有的配置
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class ACategory<T> : ACategory where T : IConfig
    {
        private Dictionary<long, IConfig> _configItems;
        
        public override string Path { set; get; }
        public override string ConfigText { get; set; }
        
        

        public override Type ConfigType
        {
            get
            {
                return typeof(T);
            }
        }

        public override IConfig TryGet(int id)
        {
            IConfig t;
            if (!_configItems.TryGetValue(id, out t))
            {
                return null;
            }

            return t;
        }

        public override IConfig GetOne()
        {
            return _configItems.Values.First();
        }

        public override IConfig[] GetAll()
        {
            return _configItems.Values.ToArray();
        }

        public List<IConfig> GetList()
        {
            return _configItems.Values.ToList();
        }

       
        public override void BeginInit()
        {
            _configItems = new Dictionary<long, IConfig>();
            CustomLogger.Log(LoggerLevel.Log,$"Attempt to load config :  {Path}");
            /*
             * 这里目前是使用反射的方式去反序列化
             * 后面可以优化为自动生成反序列化代码
             * 到每个config类，避免反射消耗
             */
            Stopwatch stopwatch = new Stopwatch();
            
            stopwatch.Start();
            foreach (string str in ConfigText.Split(new[] { "\n" }, StringSplitOptions.None))
            {
                try
                {
                    string str2 = str.Trim();
                    if (str2 == "")
                    {
                        continue;
                    }
                    
                    T config = Activator.CreateInstance<T>();
                    config = JsonHelper.FromJson<T>(str2);
                    this._configItems.Add(config.Id,config);
                    

                }
                catch (Exception e)
                {
                    throw new Exception($"load config fail: {typeof(T)}", e);
                }
            }
            stopwatch.Stop();
            CustomLogger.Log(LoggerLevel.Log,$"config load time ------{stopwatch.ElapsedMilliseconds}");
            

        }

        public override void EndInit()
        {
           
        }
    }
}