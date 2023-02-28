using Identity.Models;

namespace Identity.Services
{
	public interface IAuthentication
	{
		Task<LoginResponse> SignInAsync(LoginRequest request);
		Task<LoginResponse> SignUpAsync(RegisterRequest request);
	}
}
