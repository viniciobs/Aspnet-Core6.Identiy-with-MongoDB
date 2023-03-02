using AutoFixture;
using FluentAssertions;
using Identity.Models;

namespace Identity.Tests.Models
{
	internal class LoginResponseTest
	{
		[Test]
		public void GivenLoginResponse_WhenSuccess_ThenErrorMustBeEmptyAndNotNull()
		{
			// Arrange
			var fixture = new Fixture();
			var response = fixture.Create<LoginResponse>();

			// Act

			// Assert
			response.Errors.Should().NotBeNull().And.BeEmpty();
		}
	}
}
