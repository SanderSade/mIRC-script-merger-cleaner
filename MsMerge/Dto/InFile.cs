using MsMerge.Application;
using Newtonsoft.Json;

namespace MsMerge.Dto
{
	internal sealed class InFile
	{

		[JsonProperty("script", Required = Required.Always)]
		internal string Filename { get; set; }

		[JsonProperty("localAliases")]
		internal bool LocalAliases { get; set; }

		[JsonProperty("commentMode")]
		internal CommentMode CommentMode { get; set; }

		[JsonProperty("minify")]
		internal bool Minify { get; set; }

		[JsonProperty("stripDebug")]
		internal bool StripDebug { get; set; }
	}
}