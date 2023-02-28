using System.Text.Json.Serialization;

namespace Api.Responses
{
	public class ResponseError
	{
		public IEnumerable<string> Errors { get; init; }

		[JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
		public string ExceptionMessage { get; init; }
	}
}
