using Infra.Entities;
using Identity.Models;
using Identity.Options;
using Microsoft.AspNetCore.Identity;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace Identity.Services.Implementations
{
	public class AuthenticationService : IAuthentication
	{
		private readonly SignInManager<User> _signInManager;
		private readonly UserManager<User> _userManager;
		private readonly JwtOptions _jwtOptions;

		public AuthenticationService(
			SignInManager<User> signInManager,
			UserManager<User> userManager,
			JwtOptions jwtOptions) =>
			(_signInManager, _userManager, _jwtOptions) = (signInManager, userManager, jwtOptions);

		public async Task<LoginResponse> SignInAsync(LoginRequest request)
		{
			var user = await _userManager.FindByNameAsync(request.Username);
			if (user is null)
			{
				return new LoginResponse().WithErrors("Username not found");
			}

			var result = await _signInManager.CheckPasswordSignInAsync(
					user,
					request.Password,
					lockoutOnFailure: true);

			if (result.Succeeded is false)
			{
				return new LoginResponse().WithErrors("Invalid credentials");
			}

			return GenerateCredentialsAsync(user);
		}

		public async Task<LoginResponse> SignUpAsync(RegisterRequest request)
		{
			var identityUser = new User
			{
				UserName = request.Username,
				Email = request.Email,
				EmailConfirmed = true,
			};

			var result = await _userManager.CreateAsync(identityUser, request.Password);

			if (result.Succeeded is false)
			{
				return new LoginResponse()
					.WithErrors(result.Errors
						.Select(x => $"{x.Code}: {x.Description}")
						.ToArray());
			}

			await _userManager.SetLockoutEnabledAsync(identityUser, false);
			var user = await _userManager.FindByEmailAsync(request.Email);

			return GenerateCredentialsAsync(user);
		}

		private LoginResponse GenerateCredentialsAsync(User user)
		{
			var claims = new List<Claim>
			{
				new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
				new Claim(JwtRegisteredClaimNames.Email, user.Email),
				new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
				new Claim(JwtRegisteredClaimNames.Nbf, DateTime.Now.ToString()),
				new Claim(JwtRegisteredClaimNames.Iat, DateTime.Now.ToString())
			};

			IList<Claim> userClaims = new List<Claim>(0);
			IList<string> roles = new List<string>(0);

			Task.WaitAll(
				Task.Run(async () =>
					userClaims = await _userManager.GetClaimsAsync(user)),
				Task.Run(async () =>
					roles = await _userManager.GetRolesAsync(user))
			);

			claims.AddRange(userClaims);

			foreach (var role in roles)
			{
				claims.Add(new Claim("role", role));
			}

			var expires = DateTime.Now.AddMinutes(_jwtOptions.TokenLifetimeInMinutes);

			return new LoginResponse()
			{
				ExpiresAt = DateTime.Now.AddMinutes(_jwtOptions.TokenLifetimeInMinutes),
				AccessToken = new JwtSecurityTokenHandler()
					.WriteToken(new JwtSecurityToken(
						issuer: _jwtOptions.Issuer,
						audience: _jwtOptions.Audience,
						claims: claims,
						notBefore: DateTime.Now,
						expires: expires,
						signingCredentials: _jwtOptions.SigningCredentials))
			};
		}
	}
}
