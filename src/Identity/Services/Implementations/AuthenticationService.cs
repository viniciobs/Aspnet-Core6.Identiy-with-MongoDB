using Identity.Configurations;
using Identity.Models;
using Infra.Entities;
using Infra.Services;
using Microsoft.AspNetCore.Identity;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Transactions;

namespace Identity.Services.Implementations
{
    public class AuthenticationService : IAuthentication
	{
		private readonly SignInManager<User> _signInManager;
		private readonly UserManager<User> _userManager;
		private readonly JwtConfiguration _jwtOptions;
		private readonly IMessageProducer _messageProducer;

		public AuthenticationService(
			SignInManager<User> signInManager,
			UserManager<User> userManager,
			JwtConfiguration jwtOptions,
			IMessageProducer messageProducer) =>
			(_signInManager, _userManager, _jwtOptions, _messageProducer) =
			(signInManager, userManager, jwtOptions, messageProducer);

        public async Task<LoginResponse> ConfirmEmailAsync(EmailConfirmationRequest request)
        {
            var user = await _userManager.FindByEmailAsync(request.Email);

            if (user is null)
            {
                return new LoginResponse().WithErrors("Email not found");
            }

			var result = await _userManager.ConfirmEmailAsync(user, request.Token);

			if (result.Succeeded is false) 
			{
                return new LoginResponse()
                    .WithErrors(result.Errors
                        .Select(x => $"{x.Code}: {x.Description}")
                        .ToArray());
            }

			return GenerateCredentialsAsync(user);
        }

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

			if (result.Succeeded)
			{
				return GenerateCredentialsAsync(user);
			}

			var isEmailConfirmed = await _userManager.IsEmailConfirmedAsync(user);

			if (isEmailConfirmed is false) 
			{
                return new LoginResponse().WithErrors("Unable to login due pending e-mail confirmation");
            }

			return new LoginResponse().WithErrors("Invalid credentials");
		}

		public async Task<(bool IsSuccess, string[] Errors)> SignUpAsync(RegisterRequest request, CancellationToken cancellationToken)
		{
			var identityUser = new User
			{
				UserName = request.Username,
				Email = request.Email,
				EmailConfirmed = false,
            };

			var result = await _userManager.CreateAsync(identityUser, request.Password);

			if (result.Succeeded is false)
			{
				return (
					IsSuccess: false,
					Errors: result.Errors
						.Select(x => $"{x.Code}: {x.Description}")
						.ToArray());
			}

			var user = await _userManager.FindByEmailAsync(request.Email);
			var emailConfirmationToken = await _userManager.GenerateEmailConfirmationTokenAsync(user);

			var isConfirmationSent = await _messageProducer.SendMessageAsync(new
			{
				Email = user.Email,
				ConfirmationToken = emailConfirmationToken
			},
			cancellationToken);

			if (isConfirmationSent is false)
			{
				return (
					IsSuccess: false,
					Errors: new[] { "Unable to send e-mail confirmation. Try again." });
			}

            return (
				IsSuccess: true,
				Errors: Array.Empty<string>());
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
