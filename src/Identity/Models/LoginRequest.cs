using System.ComponentModel.DataAnnotations;

namespace Identity.Models
{
	public class LoginRequest
	{
		[Required]
		public string Username { get; init; }

		[Required, DataType(DataType.Password)]
		public string Password { get; init; }
	}
}
