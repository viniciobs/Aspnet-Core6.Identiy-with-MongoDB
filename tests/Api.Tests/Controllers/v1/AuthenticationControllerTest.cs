using Api.Controllers.v1;
using AutoFixture;
using AutoFixture.NUnit3;
using FluentAssertions;
using Identity.Models;
using Identity.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;

namespace Api.Test.Controllers.v1
{
	internal class AuthenticationControllerTest
	{
		private readonly Fixture _fixture = new();
		private AuthenticationController _sut;
		private Mock<IAuthentication> _authenticationService;

		[SetUp]
		public void Setup()
		{
			_authenticationService = new Mock<IAuthentication>(MockBehavior.Strict);
		}

		[Test, AutoData]
		public async Task GivenSignin_WhenInvalidData_ThenReturnNotFound(LoginRequest request)
		{
			// Arrange
			var errors = _fixture.Create<string[]>();
			var loginErrorResponse = _fixture.Create<LoginResponse>().WithErrors(errors);
			_authenticationService
				.Setup(x => x.SignInAsync(request))
				.ReturnsAsync(loginErrorResponse);
			_sut = GetController();

			// Act
			var result = await _sut.SignIn(request) as ObjectResult;

			// Assert
			result.Should().NotBeNull();
			result.StatusCode.Should().Be(StatusCodes.Status404NotFound);
		}

		[Test, AutoData]
		public async Task GivenSignin_WhenValidData_ThenReturnSuccessOk(LoginRequest request)
		{
			// Arrange
			var loginSuccessResponse = _fixture.Create<LoginResponse>();
			_authenticationService
				.Setup(x => x.SignInAsync(request))
				.ReturnsAsync(loginSuccessResponse);
			_sut = GetController();

			// Act
			var result = await _sut.SignIn(request) as ObjectResult;

			// Assert
			result.Should().NotBeNull();
			result.StatusCode.Should().Be(StatusCodes.Status200OK);
			result.Value.As<LoginResponse>().Errors.Should().BeEmpty();
			result.Value.As<LoginResponse>().ExpiresAt.Should().NotBeNull();
			result.Value.As<LoginResponse>().AccessToken.Should().NotBeNull().And.NotBeEmpty();
		}

		[Test, AutoData]
		public async Task GivenSignup_WhenUnableToRegister_ThenReturnUnprocessableEntity(RegisterRequest request)
		{
			// Arrange
			var errors = _fixture.Create<string[]>();
			var loginErrorResponse = _fixture.Create<LoginResponse>().WithErrors(errors);
			_authenticationService
				.Setup(x => x.SignUpAsync(request))
				.ReturnsAsync(loginErrorResponse);
			_sut = GetController();

			// Act
			var result = await _sut.SignUp(request) as ObjectResult;

			// Assert
			result.Should().NotBeNull();
			result.StatusCode.Should().Be(StatusCodes.Status422UnprocessableEntity);
		}

		[Test, AutoData]
		public async Task GivenSignup_WhenRegisterSuccessfully_ThenReturnCreated(RegisterRequest request)
		{
			// Arrange
			var loginSuccessResponse = _fixture.Create<LoginResponse>();
			_authenticationService
				.Setup(x => x.SignUpAsync(request))
				.ReturnsAsync(loginSuccessResponse);
			_sut = GetController();

			// Act
			var result = await _sut.SignUp(request) as ObjectResult;

			// Assert
			result.Should().NotBeNull();
			result.StatusCode.Should().Be(StatusCodes.Status201Created);
			result.Value.As<LoginResponse>().Errors.Should().BeEmpty();
			result.Value.As<LoginResponse>().ExpiresAt.Should().NotBeNull();
			result.Value.As<LoginResponse>().AccessToken.Should().NotBeNull().And.NotBeEmpty();
		}

		private AuthenticationController GetController() =>
			new(_authenticationService.Object);
	}
}
