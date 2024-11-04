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
}