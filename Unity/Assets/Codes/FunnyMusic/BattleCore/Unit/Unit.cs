using Framework;

namespace FunnyMusic
{
    [ChildOf(typeof(UnitComponent))]
    public class Unit : Entity, IAwake<int>
    {
        public UnitConfig Config;
       
        private float3 position; //坐标
    }
}