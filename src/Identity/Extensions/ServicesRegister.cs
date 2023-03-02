using Identity.Services;
using Identity.Services.Implementations;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Diagnostics.CodeAnalysis;

namespace Identity.Extensions
{
	[ExcludeFromCodeCoverage]
	public static class ServicesRegister
	{
		public static IServiceCollection AddIdentity(this IServiceCollection services, IConfiguration configuration) =>
			services
				.AddApiAuthentication(configuration)
				.AddScoped<IAuthentication, AuthenticationService>();
	}
}
