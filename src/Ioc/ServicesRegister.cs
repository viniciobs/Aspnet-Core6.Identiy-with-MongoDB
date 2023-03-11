using Identity.Extensions;
using Infra.Extensions;
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
				.AddExternalServices(configuration)
				.AddIdentity(configuration)
			;

			return services;
		}

	}
}
