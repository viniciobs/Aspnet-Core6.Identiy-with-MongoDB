using Api.Responses;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Diagnostics.CodeAnalysis;

namespace Api.Filters
{
	[ExcludeFromCodeCoverage]
	public class ExceptionFilter : IExceptionFilter
	{
		public void OnException(ExceptionContext context)
		{
			context.Result = new JsonResult(
				new ResponseError()
				{
					Errors = new[] { "Unexpected error" },
					ExceptionMessage = context.Exception.Message
				});
		}
	}
}
