using Infra.Entities;
using Infra.Extensions;
using Identity.Configurations;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using System.Diagnostics.CodeAnalysis;

namespace Identity.Extensions
{
	[ExcludeFromCodeCoverage]
	public static class AuthenticationExtensions
	{
		public static IServiceCollection AddApiAuthentication(this IServiceCollection services, IConfiguration configuration)
		{
			services
				.AddIdentityDatabase(configuration)
				.AddUserManager<UserManager<User>>()
				.AddSignInManager<SignInManager<User>>()
				.AddRoleManager<RoleManager<Role>>()
				.AddDefaultTokenProviders();

			var jwtConfiguration = configuration
						.GetSection(nameof(JwtConfiguration))
						.Get<JwtConfiguration>();

			services.AddSingleton(jwtConfiguration);

			services
				.Configure<IdentityOptions>(options =>
				{
					options.Password.RequireDigit = true;
					options.Password.RequireLowercase = true;
					options.Password.RequireNonAlphanumeric = true;
					options.Password.RequireUppercase = true;
					options.Password.RequiredLength = 8;

					options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
					options.Lockout.MaxFailedAccessAttempts = 3;

					options.User.RequireUniqueEmail = true;

					options.SignIn.RequireConfirmedEmail = true;
				})
				.AddAuthentication(options =>
				{
					options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
					options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
				})
				.AddJwtBearer(options =>
				{
					options.RequireHttpsMetadata = true;
					options.SaveToken = true;

					options.TokenValidationParameters = new TokenValidationParameters
					{
						ValidateIssuer = true,
						ValidIssuer = jwtConfiguration.Issuer,

						ValidateAudience = true,
						ValidAudience = jwtConfiguration.Audience,

						ValidateIssuerSigningKey = true,
						IssuerSigningKey = jwtConfiguration.SecurityKey,

						RequireExpirationTime = true,
						ValidateLifetime = true,

						ClockSkew = TimeSpan.Zero
					};
				});

			return services;
		}
	}
}
