using System.Collections.Generic;
using System.Linq;
using Core;

namespace FunnyMusic
{
    public class ConfigManager : Singleton<ConfigManager>
    {
        public List<BeatConfig> BeatConfigs;
        public List<UnitConfig> UnitConfigs;
        
        public override void Initialize()
        {
            BeatConfigCategory beatConfigCategory = ConfigGenerateComponent.Instance.AllConfig[typeof(BeatConfigCategory)] as BeatConfigCategory;
            BeatConfigs = beatConfigCategory.GetList().OfType<BeatConfig>().ToList();
            
            UnitConfigCategory unitConfigCategory = ConfigGenerateComponent.Instance.AllConfig[typeof(UnitConfigCategory)] as UnitConfigCategory;
            UnitConfigs = unitConfigCategory.GetList().OfType<UnitConfig>().ToList();
            
        }

    }
}