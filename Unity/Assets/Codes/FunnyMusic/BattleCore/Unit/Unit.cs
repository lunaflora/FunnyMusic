using Framework;

namespace FunnyMusic
{
    [ChildOf(typeof(UnitComponent))]
    public class Unit : Entity, IAwake<int>
    {
        public int ConfigId { get; set; } //配置表id
        public UnitConfig Config => ConfigManager.Instance.UnitConfigs.Find((config) => config.Id == ConfigId);
       
        private float3 position; //坐标
    }
    
    public class UnitSystem: AwakeSystem<Unit, int>
    {
        public override void Awake(Unit self, int configId)
        {
            self.ConfigId = configId;
        }
    }
}