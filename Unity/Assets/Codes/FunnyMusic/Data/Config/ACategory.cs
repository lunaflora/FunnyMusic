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

                    //CustomLogger.Log(LoggerLevel.Log,$"{config.Id}");

                    /*JsonData josnObj = JsonHelper.FromJson(str2);
 
                    T config = Activator.CreateInstance<T>();
                    var type = config.GetType();
                    var fieldsInfo = type.GetFields(BindingFlags.Instance | BindingFlags.Public);
                    foreach (var field in fieldsInfo)
                    {
                        if (josnObj.ContainsKey(field.Name))
                        {
                            //Debug.Log(field.FieldType);
                            if (field.FieldType == typeof(long))
                            {
                                long configNumber = 0;
                                if (!long.TryParse(josnObj[field.Name].ToString(), out configNumber))
                                {
                                    CustomLogger.Log(LoggerLevel.Error,$"{type} config Id {config.Id} field {field.Name} format error!");
                                    
                                }
                                field.SetValue(config,configNumber);
                            }
                            if (field.FieldType == typeof(ConfigArray<long>))
                            {
                                var arry = josnObj[field.Name];
                                ConfigArray<long> configArray = new long[arry.Count];
                                int index = 0;
                                foreach (var value in arry)
                                {
                                    long configNumber = 0;
                                    if(!long.TryParse(value.ToString(), out configNumber))
                                    {
                                        CustomLogger.Log(LoggerLevel.Error,$"{type} config Id {config.Id} field {field.Name} format error!");
                                    
                                    }

                                    configArray[index] = configNumber;
                                    index++;

                                }
                                field.SetValue(config,configArray);
                            }

                            if (field.FieldType == typeof(int))
                            {
                                int configNumber = 0;
                                if (!int.TryParse(josnObj[field.Name].ToString(), out configNumber))
                                {
                                    CustomLogger.Log(LoggerLevel.Error,$"{type} config Id {config.Id} field {field.Name} format error!");
                                    
                                }
                                field.SetValue(config,configNumber);
                            }
                            
                            if (field.FieldType == typeof(ConfigArray<int>))
                            {
                                var arry = josnObj[field.Name];
                                ConfigArray<int> configArray = new int[arry.Count];
                                int index = 0;
                                foreach (var value in arry)
                                {
                                    int configNumber = 0;
                                    if(!int.TryParse(value.ToString(), out configNumber))
                                    {
                                        CustomLogger.Log(LoggerLevel.Error,$"{type} config Id {config.Id} field {field.Name} format error!");
                                    
                                    }

                                    configArray[index] = configNumber;
                                    index++;

                                }
                                field.SetValue(config,configArray);
                            }
                            
                            
                            if (field.FieldType == typeof(float))
                            {
                                float configNumber = 0f;
                                if (!float.TryParse(josnObj[field.Name].ToString(), out configNumber))
                                {
                                    CustomLogger.Log(LoggerLevel.Error,$"{type} config Id {config.Id} field {field.Name} format error!");
                                    
                                }
                                field.SetValue(config,configNumber);
                            }
                            if (field.FieldType == typeof(double))
                            {
                                double configNumber = 0f;
                                if (!double.TryParse(josnObj[field.Name].ToString(), out configNumber))
                                {
                                    CustomLogger.Log(LoggerLevel.Error,$"{type} config Id {config.Id} field {field.Name} format error!");
                                    
                                }
                                field.SetValue(config,configNumber);
                            }
                            
                            if (field.FieldType == typeof(string))
                            {
                                string configNumber = "";
                                configNumber = josnObj[field.Name].ToString();
                              
                                field.SetValue(config,configNumber);
                            }
                            
                            if (field.FieldType == typeof(ConfigArray<string>))
                            {
                                var arry = josnObj[field.Name];
                                ConfigArray<string> configArray = new String[arry.Count];
                                int index = 0;
                                foreach (var value in arry)
                                {
                                    string configNumber = "";
                                    configNumber = value.ToString();

                                    configArray[index] = configNumber;
                                    index++;

                                }
                                field.SetValue(config,configArray);
                            }

                            if (field.FieldType == typeof(FixedNumber))
                            {
                                long configNumber = 0;
                                if (!long.TryParse(josnObj[field.Name].ToString(), out configNumber))
                                {
                                    CustomLogger.Log(LoggerLevel.Error,$"{type} config Id {config.Id} field {field.Name} format error!");
                                    
                                }

                                FixedNumber fixedNumber = FixedNumber.MakeFixNum(configNumber, GlobalMagicNumber.ConfigFixNumberScale);
                                field.SetValue(config,fixedNumber);

                            }
                            
                            if (field.FieldType == typeof(ConfigArray<FixedNumber>))
                            {
                                var arry = josnObj[field.Name];
                                ConfigArray<FixedNumber> configArray = new FixedNumber[arry.Count];
                                int index = 0;
                                foreach (var value in arry)
                                {
                                    long configNumber = 0;
                                    if(!long.TryParse(value.ToString(), out configNumber))
                                    {
                                        CustomLogger.Log(LoggerLevel.Error,$"{type} config Id {config.Id} field {field.Name} format error!");
                                    
                                    }

                                    configArray[index] = FixedNumber.MakeFixNum(configNumber, GlobalMagicNumber.ConfigFixNumberScale);

                                    index++;
                                }
                                field.SetValue(config,configArray);
                                CustomLogger.Log(LoggerLevel.Log,configArray.ToString());

                            }
                            


                        }
                    }*/



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