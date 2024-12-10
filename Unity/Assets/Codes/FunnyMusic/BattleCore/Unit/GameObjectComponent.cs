using Core;
using Cysharp.Threading.Tasks;
using Framework;
using UnityEngine;

namespace FunnyMusic
{
    [ComponentOf(typeof(Unit))]
    public class GameObjectComponent: Entity, IAwake, IDestroy
    {
        public GameObject GameObject { get; set; }
        public string AssetPath;
    }
    
    
    
    [ObjectSystem]
    public class GameObjectComponentDestroySystem : DestroySystem<GameObjectComponent>
    {
        public override void Destroy(GameObjectComponent self)
        {
            AssetLoaderSystem.Instance.UnloadAsset(self.AssetPath);
        }
    }


    public static class GameObjectComponentSystem
    {
        public static async UniTask LoadGameObject(this GameObjectComponent self)
        {
            var unitConfig = self.GetParent<Unit>().Config;
            self.AssetPath = string.Format(ResourcesPath.InternalUnitGameObjectPath,unitConfig.Prefab,unitConfig.Prefab);
            self.GameObject = await AssetLoaderSystem.Instance.InstantiateAsync(
                self.AssetPath,GlobalGameObjectComponent.Instance.UnitRoot);
            //RhythmCoreUtil.ConvertIntToFloat
            self.GameObject.transform.position = new Vector3(RhythmCoreUtil.ConvertIntToFloat(unitConfig.Position[0]),
                RhythmCoreUtil.ConvertIntToFloat(unitConfig.Position[1]),
                RhythmCoreUtil.ConvertIntToFloat(unitConfig.Position[2]));
            
            self.GameObject.transform.rotation = Quaternion.Euler(0,(float)unitConfig.Yaw,0);

        }
    }
}