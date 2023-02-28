namespace Identity.Models
{
	public class LoginResponse
	{
		public string AccessToken { get; init; }
		public DateTime? ExpiresAt { get; init; }
		public string[] Errors { get; private set; } = Array.Empty<string>();

		public LoginResponse WithErrors(params string[] errors)
		{
			Errors = errors;

			return this;
		}
	}
}
