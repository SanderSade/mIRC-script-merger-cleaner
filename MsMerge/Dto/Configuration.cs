using System.Collections.Generic;
using Newtonsoft.Json;

namespace MsMerge.Dto
{
	internal sealed class Configuration
	{
		[JsonProperty("input", Required = Required.Always)]
		internal List<InFile> Infiles { get; set; }

		[JsonProperty("output", Required = Required.Always)]
		internal string OutFile { get; set; }

		[JsonIgnore]
		public bool ConsoleOut { get; set; }
	}
}