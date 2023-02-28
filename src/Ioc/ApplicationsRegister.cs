using Ioc.Extensions;
using Microsoft.AspNetCore.Builder;

namespace Ioc
{
	public static class ApplicationsRegister
	{
		public static void UseSwaggerInterface(this WebApplication app) =>
			app.AddSwagger();
	}
}
