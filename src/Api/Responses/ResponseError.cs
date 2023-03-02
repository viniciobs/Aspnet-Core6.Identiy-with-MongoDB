using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Serialization;

namespace Api.Responses
{
	[ExcludeFromCodeCoverage]
	public class ResponseError
	{
		public IEnumerable<string> Errors { get; init; }

		[JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
		public string ExceptionMessage { get; init; }
	}
}
