/*-----------------------------------------------------------------
    以下代码为自动生成的代码，任何对以下代码的修改将在下次自动生成后被覆
    盖，不要对以下代码进行修改
-----------------------------------------------------------------*/

using Framework;
using System.Collections.Generic;

namespace FunnyMusic
{
	[Config("ServerConfig")]
	public partial class ServerConfigCategory : ACategory<ServerConfig>
	{
	}

	public class ServerConfig: IConfig
	{
		public int ServerGroup;
		public int ServerId;
		public List<int> ConList;
	}
}
