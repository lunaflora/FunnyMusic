/*-----------------------------------------------------------------
    以下代码为自动生成的代码，任何对以下代码的修改将在下次自动生成后被覆
    盖，不要对以下代码进行修改
-----------------------------------------------------------------*/

using Framework;
using System.Collections.Generic;

namespace FunnyMusic
{
	[Config("BeatConfig")]
	public partial class BeatConfigCategory : ACategory<BeatConfig>
	{
	}

	public class BeatConfig: IConfig
	{
		public int TrackID;
		public int BeatType;
		public List<int> TriggerRange;
		public string Prefab;
		public string Desc;
	}
}
