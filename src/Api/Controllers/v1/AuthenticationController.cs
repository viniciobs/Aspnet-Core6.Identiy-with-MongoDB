using Api.Responses;
using Identity.Models;
using Identity.Services;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers.v1
{
    [ApiController]
	[ApiVersion("1.0")]
	[Route("/api/v1/")]
	[Produces("application/json")]
	public class AuthenticationController : ControllerBase
	{
		private readonly IAuthentication _authenticationService;

		public AuthenticationController(IAuthentication authenticationService) =>
			_authenticationService = authenticationService;

		[HttpPost]
		[Route("signin")]
		[ProducesResponseType(StatusCodes.Status200OK, Type = typeof(LoginResponse))]
		[ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ResponseError))]
		[ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ResponseError))]
		[ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(ResponseError))]
		public async Task<IActionResult> SignIn([FromBody] LoginRequest request)
		{
			var result = await _authenticationService.SignInAsync(request);

			if (result.Errors.Any())
			{
				return NotFound(result.Errors);
			}

			return Ok(result);
		}

		[HttpPost]
		[Route("signup")]
		[ProducesResponseType(StatusCodes.Status201Created)]
		[ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ResponseError))]
		[ProducesResponseType(StatusCodes.Status422UnprocessableEntity, Type = typeof(ResponseError))]
		[ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(ResponseError))]
		public async Task<IActionResult> SignUp([FromBody] RegisterRequest request, CancellationToken cancellationToken)
		{
			var result = await _authenticationService.SignUpAsync(request, cancellationToken);

			if (result.IsSuccess is false)
			{
				return UnprocessableEntity(result.Errors);
			}

			return StatusCode(StatusCodes.Status201Created);
		}

        [HttpGet]
        [Route("confirm-email")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(LoginResponse))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ResponseError))]
        [ProducesResponseType(StatusCodes.Status422UnprocessableEntity, Type = typeof(ResponseError))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(ResponseError))]
        public async Task<IActionResult> ConfirmEmail([FromQuery] Guid userId, string token)
		{
			var result = await _authenticationService.ConfirmEmailAsync(
				new EmailConfirmationRequest 
				{ 
					UserId = userId,
					Token = token,
				});

			if (result.Errors.Any())
			{
				return UnprocessableEntity(result.Errors);
			}

            return StatusCode(StatusCodes.Status201Created, result);
        }
	}
}
