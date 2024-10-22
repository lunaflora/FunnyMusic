using System.Collections.Generic;
using Framework;
using System;
using System.Threading.Tasks;
using Core;
using Cysharp.Threading.Tasks;
using FLib;
using UnityEngine;

namespace FunnyMusic
{
    public class ConfigGenerateComponent : Entity,IAwake,IDestroy,IUpdate
    {
        public static ConfigGenerateComponent Instance;
        
        public readonly Dictionary<Type, ACategory> AllConfig = new Dictionary<Type, ACategory>();
        

    }
    
    [ObjectSystem]
    public class ConfigGenerateAwakeSystem : AwakeSystem<ConfigGenerateComponent>
    {
        public override void Awake(ConfigGenerateComponent self)
        {
            ConfigGenerateComponent.Instance = self;
          
        }
    }

    [ObjectSystem]
    public class ConfigGenerateDestroySystem : DestroySystem<ConfigGenerateComponent>
    {
        public override void Destroy(ConfigGenerateComponent self)
        {
            ConfigGenerateComponent.Instance = null;
        }
    }
    
    [ObjectSystem]
    public class ConfigGenerateUpdateSystem : UpdateSystem<ConfigGenerateComponent>
    {
        public override void Update(ConfigGenerateComponent self)
        {
            self.UpdateLoadingProgress();
        }
    }

    [FriendClass(typeof(ConfigGenerateComponent))]
    public static class ConfigGenerateSystem
    {


        public async static UniTask LoadOneConfig(this ConfigGenerateComponent self, Type configType, string path)
        {
            var instance = Activator.CreateInstance(configType);
            ACategory aCategory = instance as ACategory;
            TextAsset textResult =
                await AssetLoaderSystem.Instance.LoadAssetAsync<TextAsset>(
                    string.Format(ResourcesPath.InternalConfigPath, path));
            aCategory.ConfigText = textResult.text;
            aCategory.Path = path;
            aCategory.BeginInit();
            aCategory.EndInit();


            FDebug.Print(LoggerLevel.Log, $"Loaded config type : {configType}");
            self.AllConfig[configType] = aCategory;

            AssetLoaderSystem.Instance.UnloadAsset(string.Format(ResourcesPath.InternalConfigPath, path));
        

        }

        public static void LoadOneConfigInThread(this ConfigGenerateComponent self, Type configType, string path)
        {
            var instance = Activator.CreateInstance(configType);
            ACategory aCategory = instance as ACategory;
            aCategory.Path = path;
            aCategory.BeginInit();
            aCategory.EndInit();


            CustomLogger.Log(LoggerLevel.Log, $"Loaded config type : {configType}");
            lock (self)
            {
                self.AllConfig[configType] = aCategory;


            }


        }

        public static void LoadAll(this ConfigGenerateComponent self)
        {
            self.AllConfig.Clear();

            List<Type> types = WorldSystem.Instance.GetTypes(typeof(ConfigAttribute));

            foreach (var configType in types)
            {
                object[] objects = configType.GetCustomAttributes(typeof(ConfigAttribute), false);
                if (objects.Length == 0)
                {
                    continue;
                }

                ConfigAttribute baseAttribute = (ConfigAttribute)objects[0];
                var path = baseAttribute.Path;

                self.LoadOneConfig(configType, path).Forget();

            }

        }

        public static async UniTask LoadAllAsync(this ConfigGenerateComponent self)
        {

            self.AllConfig.Clear();


            List<Type> types = WorldSystem.Instance.GetTypes(typeof(ConfigAttribute));


            foreach (var configType in types)
            {
                object[] objects = configType.GetCustomAttributes(typeof(ConfigAttribute), false);
                if (objects.Length == 0)
                {
                    continue;
                }

                ConfigAttribute baseAttribute = (ConfigAttribute)objects[0];
                var path = baseAttribute.Path;

                await self.LoadOneConfig(configType, path);
            }

            self.UpdateLoadingProgress();




        }

        /// <summary>
        /// 这里试一下多线程加载
        /// 主要适用于从外部读取配置
        /// 不能用unity的接口
        /// </summary>
        /// <param name="self"></param>
        public static async UniTask LoadAllMultiThread(this ConfigGenerateComponent self)
        {

            self.AllConfig.Clear();

            List<Type> types = WorldSystem.Instance.GetTypes(typeof(ConfigAttribute));
            using (ListComponent<Task> listTasks = ListComponent<Task>.Create())
            {
                foreach (var configType in types)
                {
                    object[] objects = configType.GetCustomAttributes(typeof(ConfigAttribute), false);
                    if (objects.Length == 0)
                    {
                        continue;
                    }

                    ConfigAttribute baseAttribute = (ConfigAttribute)objects[0];
                    var path = baseAttribute.Path;

                    Task task = Task.Run(() => self.LoadOneConfigInThread(configType, path));
                    listTasks.Add(task);


                }

                await Task.WhenAll(listTasks.ToArray());

            }



        }
        

        public static void UpdateLoadingProgress(this ConfigGenerateComponent self)
        {

        }

    }

}