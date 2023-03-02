using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace Identity.Models
{
	[ExcludeFromCodeCoverage]
	public class RegisterRequest
	{
		[Required, EmailAddress]
		public string Email { get; init; }

		[Required]
		public string Username { get; init; }

		[Required, DataType(DataType.Password)]
		public string Password { get; init; }

		[Required, DataType(DataType.Password)]
		[Compare(nameof(Password), ErrorMessage = "The password and confirmation password mismatch")]
		public string ConfirmPassword { get; init; }
	}
}
