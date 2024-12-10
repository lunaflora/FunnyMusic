using Framework;

namespace FunnyMusic
{
    /// <summary>
    /// 战斗模块单位管理器
    /// </summary>
    [ComponentOf(typeof(World))]
    [ChildOf(typeof(UnitComponent))]
    public class UnitComponent : Entity, IAwake, IDestroy
    {
        
    }
    
    [ObjectSystem]
    public class UnitComponentAwakeSystem : AwakeSystem<UnitComponent>
    {
        public override void Awake(UnitComponent self)
        {
        }
    }
	
    [ObjectSystem]
    public class UnitComponentDestroySystem : DestroySystem<UnitComponent>
    {
        public override void Destroy(UnitComponent self)
        {
        }
    }

    public static class UnitComponentSystem
    {
        public static void Add(this UnitComponent self, Unit unit)
        {
        }

        public static Unit Get(this UnitComponent self, long id)
        {
            Unit unit = self.GetChild<Unit>(id);
            return unit;
        }

        public static void Remove(this UnitComponent self, long id)
        {
            Unit unit = self.GetChild<Unit>(id);
            unit?.Dispose();
        }
    }
}