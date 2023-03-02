using Api.Responses;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Diagnostics.CodeAnalysis;

namespace Api.Filters
{
	[ExcludeFromCodeCoverage]
	public class ActionFilter : IActionFilter
	{
		public void OnActionExecuting(ActionExecutingContext context)
		{
			if (context.ModelState.IsValid is false)
			{
				context.Result = new BadRequestObjectResult(
					new ResponseError()
					{
						Errors = context.ModelState.Values
							.SelectMany(x => x.Errors)
							.Select(x => x.ErrorMessage)
					});
			}
		}

		public void OnActionExecuted(ActionExecutedContext context)
		{
			if (context.Result is not ObjectResult result)
			{
				return;
			}

			if (result.StatusCode is < 200 or >= 300)
			{
				var responseError = new ResponseError()
				{
					Errors = result.Value as IEnumerable<string>
				};

				context.Result = new JsonResult(responseError)
				{
					StatusCode = result.StatusCode
				};
			}
		}
	}
}
