/*-----------------------------------------------------------------
    以下代码为自动生成的代码，任何对以下代码的修改将在下次自动生成后被覆
    盖，不要对以下代码进行修改
-----------------------------------------------------------------*/

using Framework;
using System.Collections.Generic;

namespace FunnyMusic
{
	[Config("AIConfig")]
	public partial class AIConfigCategory : ACategory<AIConfig>
	{
	}

	public class AIConfig: IConfig
	{
		public int AIConfigId;
		public int Order;
		public float AngularSpeed;
		public int ChaseRange;
		public List<int> SpeedSteps;
		public string Name;
		public List<string> NameArray;
		public List<int> NodeParams;
	}
}
