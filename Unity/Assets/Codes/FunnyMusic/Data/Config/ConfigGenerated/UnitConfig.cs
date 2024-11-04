/*-----------------------------------------------------------------
    以下代码为自动生成的代码，任何对以下代码的修改将在下次自动生成后被覆
    盖，不要对以下代码进行修改
-----------------------------------------------------------------*/

using Framework;
using System.Collections.Generic;

namespace FunnyMusic
{
	[Config("UnitConfig")]
	public partial class UnitConfigCategory : ACategory<UnitConfig>
	{
	}

	public class UnitConfig: IConfig
	{
		public int Type;
		public string Name;
		public string Desc;
		public int Position;
		public int Height;
		public int Weight;
	}
}
