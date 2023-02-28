using Identity.Services;
using Identity.Services.Implementations;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Identity.Extensions
{
	public static class ServicesRegister
	{
		public static IServiceCollection AddIdentity(this IServiceCollection services, IConfiguration configuration) =>
			services
				.AddApiAuthentication(configuration)
				.AddScoped<IAuthentication, AuthenticationService>();
	}
}
