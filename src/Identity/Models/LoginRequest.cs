using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace Identity.Models
{
	[ExcludeFromCodeCoverage]
	public class LoginRequest
	{
		[Required]
		public string Username { get; init; }

		[Required, DataType(DataType.Password)]
		public string Password { get; init; }
	}
}
