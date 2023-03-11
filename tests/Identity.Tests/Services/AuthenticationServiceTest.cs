using AutoFixture;
using AutoFixture.NUnit3;
using FluentAssertions;
using Identity.Models;
using Identity.Configurations;
using Identity.Services;
using Identity.Services.Implementations;
using Infra.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using Moq;
using System.Security.Claims;
using Infra.Services;

namespace Identity.Tests.Services
{
	internal class AuthenticationServiceTest
	{
		private Fixture _fixture = new();

		private IAuthentication _sut;
		
		private Mock<IUserStore<User>> _userStore;
		private Mock<IHttpContextAccessor> _contextAccessor;
		private Mock<IUserClaimsPrincipalFactory<User>> _claimsPrincipal;
		private Mock<UserManager<User>> _userManager;
		private Mock<SignInManager<User>> _signinManager;
		private Mock<JwtConfiguration> _jwtOptions;
		private Mock<IMessageProducer> _messageProducer;

		[SetUp]
		public void Setup()
		{
			_userStore = new Mock<IUserStore<User>>();
			_contextAccessor = new Mock<IHttpContextAccessor>();
			_claimsPrincipal = new Mock<IUserClaimsPrincipalFactory<User>>();
			_userManager = new Mock<UserManager<User>>(_userStore.Object, null, null, null, null, null, null, null, null);
			_signinManager = new Mock<SignInManager<User>>(_userManager.Object, _contextAccessor.Object, _claimsPrincipal.Object, null, null, null, null);
			_jwtOptions = new Mock<JwtConfiguration>();
			_messageProducer = new Mock<IMessageProducer>(MockBehavior.Strict);
		}

		[Test, AutoData]
		public async Task GivenSignin_WhenUserNotFoundByUsername_ThenReturnResponseWithError(LoginRequest request)
		{
			// Arrange
			_userManager
				.Setup(x => x.FindByNameAsync(request.Username))
				.ReturnsAsync(default(User?));
			
			_sut = GetService();

			// Act
			var result = await _sut.SignInAsync(request);

			// Assert
			result.AccessToken.Should().BeNull();
			result.ExpiresAt.Should().BeNull();
			result.Errors.Should().NotBeNull().And.HaveCountGreaterThan(0);

			_userManager
				.Verify(
					x => x.FindByNameAsync(request.Username),
					Times.Once);
			_signinManager
				.Verify(
					x => x.CheckPasswordSignInAsync(It.IsAny<User>(), request.Password, It.IsAny<bool>()),
					Times.Never);
		}

		[Test, AutoData]
		public async Task GivenSignin_WhenPasswordNotCheckked_ThenReturnResponseWithError(LoginRequest request)
		{
			// Arrange
			var user = _fixture.Create<User>();

			_userManager
				.Setup(x => x.FindByNameAsync(request.Username))
				.ReturnsAsync(user);
			_signinManager
				.Setup(x => x.CheckPasswordSignInAsync(user, request.Password, It.IsAny<bool>()))
				.ReturnsAsync(SignInResult.Failed);

			_sut = GetService();

			// Act
			var result = await _sut.SignInAsync(request);

			// Assert
			result.AccessToken.Should().BeNull();
			result.ExpiresAt.Should().BeNull();
			result.Errors.Should().NotBeNull().And.HaveCountGreaterThan(0);

			_userManager
				.Verify(
					x => x.FindByNameAsync(request.Username),
					Times.Once);
			_signinManager
				.Verify(
					x => x.CheckPasswordSignInAsync(user, request.Password, It.IsAny<bool>()),
					Times.Once);
		}

		[Test]
		public async Task GivenSignin_WhenIsAllValid_ThenReturnResponseSuccess()
		{
			// Arrange
			var user = _fixture.Create<User>();
			var request = _fixture.Build<LoginRequest>()
				.With(x => x.Username, user.UserName)
				.Create();
			var jwtOptions = _fixture.Build<JwtConfiguration>()
				.With(x => x.TokenLifetimeInMinutes, 1)
				.With(x => x.SecurityKey, new Mock<SecurityKey>().Object)
				.Create();

			_userManager
				.Setup(x => x.FindByNameAsync(request.Username))
				.ReturnsAsync(user);
			_userManager
				.Setup(x => x.GetClaimsAsync(user))
				.ReturnsAsync(new List<Claim>(0));
			_userManager
				.Setup(x => x.GetRolesAsync(user))
				.ReturnsAsync(new List<string>(0));
			_signinManager
				.Setup(x => x.CheckPasswordSignInAsync(user, request.Password, It.IsAny<bool>()))
				.ReturnsAsync(SignInResult.Success);

			_sut = GetService(jwtOptions);

			// Act
			var result = await _sut.SignInAsync(request);

			// Assert
			result.AccessToken.Should().NotBeNull();
			result.ExpiresAt.Should().NotBeNull();
			result.Errors.Should().BeEmpty();

			_userManager
				.Verify(
					x => x.FindByNameAsync(request.Username),
					Times.Once);
			_signinManager
				.Verify(
					x => x.CheckPasswordSignInAsync(user, request.Password, It.IsAny<bool>()),
					Times.Once);
			_userManager
				.Verify(
					x => x.GetClaimsAsync(user),
					Times.Once);
			_userManager
				.Verify(
					x => x.GetRolesAsync(user),
					Times.Once);
		}

		[Test, AutoData]
		public async Task GivenSignup_WhenCreateFail_ThenReturnResponseWithError(RegisterRequest request)
		{
			// Arrange
			_userManager
				.Setup(x => x.CreateAsync(It.IsAny<User>(), request.Password))
				.ReturnsAsync(IdentityResult.Failed(_fixture.Create<IdentityError>()));

			_sut = GetService();

			// Act
			var result = await _sut.SignUpAsync(request, CancellationToken.None);

			// Assert
			result.IsSuccess.Should().BeFalse();
			result.Errors.Should().NotBeNull().And.HaveCountGreaterThan(0);

			_userManager
				.Verify(
					x => x.CreateAsync(It.IsAny<User>(), request.Password),
					Times.Once);
			_userManager
				.Verify(
					x => x.FindByEmailAsync(request.Email),
					Times.Never);
			_userManager
				.Verify(
					x => x.GetClaimsAsync(It.IsAny<User>()),
					Times.Never);
			_userManager
				.Verify(
					x => x.GetRolesAsync(It.IsAny<User>()),
					Times.Never);
		}

        [Test, AutoData]
        public async Task GivenSignup_WhenCreateSuccessButUnableToSendMailConfirmation_ThenReturnResponseWithError(RegisterRequest request)
        {
            // Arrange
            var user = _fixture.Build<User>()
                .With(x => x.UserName, request.Username)
                .With(x => x.Email, request.Email)
                .Create();
            var jwtOptions = _fixture.Build<JwtConfiguration>()
                .With(x => x.TokenLifetimeInMinutes, 1)
                .With(x => x.SecurityKey, new Mock<SecurityKey>().Object)
                .Create();

            _userManager
                .Setup(x => x.CreateAsync(It.IsAny<User>(), request.Password))
                .ReturnsAsync(IdentityResult.Success);
            _userManager
                .Setup(x => x.GetClaimsAsync(user))
                .ReturnsAsync(new List<Claim>(0));
            _userManager
                .Setup(x => x.GetRolesAsync(user))
                .ReturnsAsync(new List<string>(0));
            _userManager
                .Setup(x => x.FindByEmailAsync(request.Email))
                .ReturnsAsync(user);
            _messageProducer
                .Setup(x => x.SendMessageAsync(It.IsAny<object>(), CancellationToken.None))
                .ReturnsAsync(false);

            _sut = GetService(jwtOptions);

            // Act
            var result = await _sut.SignUpAsync(request, CancellationToken.None);

            // Assert
            result.IsSuccess.Should().BeFalse();
            result.Errors.Should().NotBeEmpty();

            _userManager
                .Verify(
                    x => x.CreateAsync(It.IsAny<User>(), request.Password),
                    Times.Once);
            _userManager
                .Verify(
                    x => x.FindByEmailAsync(request.Email),
                    Times.Once);
			_messageProducer
				.Verify(
					x => x.SendMessageAsync(It.IsAny<object>(), CancellationToken.None),
					Times.Once);
		}

        [Test, AutoData]
		public async Task GivenSignup_WhenCreateSuccess_ThenReturnResponseSuccess(RegisterRequest request)
		{
			// Arrange
			var user = _fixture.Build<User>()
				.With(x => x.UserName, request.Username)
				.With(x => x.Email, request.Email)
				.Create();
			var jwtOptions = _fixture.Build<JwtConfiguration>()
				.With(x => x.TokenLifetimeInMinutes, 1)
				.With(x => x.SecurityKey, new Mock<SecurityKey>().Object)
				.Create();

			_userManager
				.Setup(x => x.CreateAsync(It.IsAny<User>(), request.Password))
				.ReturnsAsync(IdentityResult.Success);
			_userManager
				.Setup(x => x.GetClaimsAsync(user))
				.ReturnsAsync(new List<Claim>(0));
			_userManager
				.Setup(x => x.GetRolesAsync(user))
				.ReturnsAsync(new List<string>(0));
			_userManager
				.Setup(x => x.FindByEmailAsync(request.Email))
				.ReturnsAsync(user);
			_messageProducer
				.Setup(x => x.SendMessageAsync(It.IsAny<object>(), CancellationToken.None))
				.ReturnsAsync(true);

			_sut = GetService(jwtOptions);

			// Act
			var result = await _sut.SignUpAsync(request, CancellationToken.None);

			// Assert
			result.IsSuccess.Should().BeTrue();
			result.Errors.Should().BeEmpty();

			_userManager
				.Verify(
					x => x.CreateAsync(It.IsAny<User>(), request.Password),
					Times.Once);
			_userManager
				.Verify(
					x => x.FindByEmailAsync(request.Email),
					Times.Once);
            _messageProducer
                .Verify(
                    x => x.SendMessageAsync(It.IsAny<object>(), CancellationToken.None),
                    Times.Once);
        }

		private IAuthentication GetService(JwtConfiguration? jwtOptions = null) =>
			new AuthenticationService(
				_signinManager.Object,
				_userManager.Object,
				(jwtOptions ?? _jwtOptions.Object),
				_messageProducer.Object);
	}
}
