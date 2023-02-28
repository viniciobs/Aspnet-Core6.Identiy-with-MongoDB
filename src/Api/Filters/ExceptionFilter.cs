using Api.Responses;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Api.Filters
{
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
