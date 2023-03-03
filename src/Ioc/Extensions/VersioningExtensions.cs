using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;

namespace Ioc.Extensions
{
	public static class VersioningExtensions
	{
		public static IServiceCollection AddVersioning(this IServiceCollection services) =>
			services
				.AddApiVersioning(options =>
				{
					options.DefaultApiVersion = new ApiVersion(1, 0);
					options.AssumeDefaultVersionWhenUnspecified = true;
					options.ReportApiVersions = true;
				})
				.AddVersionedApiExplorer(options =>
				{
					options.GroupNameFormat = "'v'VVV";
				});
	}
}
