using Identity.Models;

namespace Identity.Services
{
	public interface IAuthentication
	{
		Task<LoginResponse> SignInAsync(LoginRequest request);
		Task<LoginResponse> ConfirmEmailAsync(EmailConfirmationRequest request);
		Task<(bool IsSuccess, string[] Errors)> SignUpAsync(RegisterRequest request, CancellationToken cancellationToken);
	}
}
