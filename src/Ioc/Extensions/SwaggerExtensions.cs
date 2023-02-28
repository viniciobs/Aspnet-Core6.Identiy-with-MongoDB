using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerUI;
using System.Reflection;


namespace Ioc.Extensions
{
	public static class SwaggerExtensions
	{
		public static IServiceCollection AddSwagger(this IServiceCollection services) =>
			services
				.AddEndpointsApiExplorer()
				.AddSwaggerGen(options =>
				{
					options.IncludeXmlComments(
						Path.Combine(
							AppContext.BaseDirectory,
							$"{Assembly.GetEntryAssembly().GetName().Name}.xml"));

					options.SwaggerDoc("v1", new OpenApiInfo
					{
						Title = "Agendamento on-line",
						Version = "v1",
					});

					options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
					{
						Name = "Authorization",
						In = ParameterLocation.Header,
						Type = SecuritySchemeType.ApiKey,
						Scheme = "Bearer"

					});

					options.AddSecurityRequirement(new OpenApiSecurityRequirement()
					{
						 {
							new OpenApiSecurityScheme
							{
								Reference = new OpenApiReference
								{
									Type = ReferenceType.SecurityScheme,
									Id = "Bearer"
								},
								Scheme = "oauth2",
								Name = "Bearer",
								In = ParameterLocation.Header,
							},
							new List<string>()
						 }
					});
				});

		public static void AddSwagger(this WebApplication app)
		{
			app.UseSwagger();
			app.UseSwaggerUI(options =>
			{
				var apiVersionProvider = app.Services.GetService<IApiVersionDescriptionProvider>()
					?? throw new ArgumentException("API Versioning not registered.");

				foreach (var description in apiVersionProvider.ApiVersionDescriptions)
				{
					options.SwaggerEndpoint(
						$"/swagger/{description.GroupName}/swagger.json",
						description.GroupName);
				}

				options.RoutePrefix = string.Empty;
				options.DocExpansion(DocExpansion.List);
			});
		}
	}
}
