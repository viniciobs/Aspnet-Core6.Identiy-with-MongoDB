using Identity.Extensions;
using Ioc.Extensions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Ioc
{
	public static class ServicesRegister
	{
		public static IServiceCollection RegisterServices(
			this IServiceCollection services,
			IConfiguration configuration)
		{
			services
				.AddSwagger()
				.AddIdentity(configuration);

			return services;
		}

	}
}
