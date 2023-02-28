using System.ComponentModel.DataAnnotations;

namespace Identity.Models
{
	public class LoginRequest
	{
		[Required(ErrorMessage = "Username is required")]
		public string Username { get; init; }

		[Required(ErrorMessage = "Password is required"), DataType(DataType.Password)]
		public string Password { get; init; }
	}
}
