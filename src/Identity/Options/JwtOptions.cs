using Microsoft.IdentityModel.Tokens;
using System.Diagnostics.CodeAnalysis;
using System.Text;

namespace Identity.Options
{
	[ExcludeFromCodeCoverage]
	public class JwtOptions
	{
		private string _secretKey;
		private SigningCredentials _signingCredentials;

		public SecurityKey SecurityKey { get; init; }
		public string Issuer { get; init; }
		public string Audience { get; init; }
		public int TokenLifetimeInMinutes { get; init; }

		private string SecretKey
		{
			get => _secretKey;
			init
			{
				_secretKey = value;
				SecurityKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(_secretKey));
			}
		}

		public SigningCredentials SigningCredentials
		{
			get
			{
				if (_signingCredentials is null && SecretKey is not null)
				{
					_signingCredentials = new SigningCredentials(
						SecurityKey,
						SecurityAlgorithms.HmacSha512);
				}

				return _signingCredentials;
			}
		}
	}
}
