using Ioc.Extensions;
using Api.Filters;
using Ioc;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddHealthChecks();
builder.Services
	.AddCors()
	.AddRouting(options =>
	{
		options.LowercaseUrls = true;
	})
	.AddVersioning()
	.RegisterServices(builder.Configuration)
	.AddControllers(options =>
	{
		options.Filters.Add(typeof(ExceptionFilter));
		options.Filters.Add(typeof(ActionFilter));
	})
	.ConfigureApiBehaviorOptions(options =>
	{
		options.SuppressModelStateInvalidFilter = true;
	});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
	app.UseSwaggerInterface();
}

app
	.UseHttpsRedirection()
	.UseAuthentication()
	.UseAuthorization()
	.UseCors(builder =>
		builder
			.SetIsOriginAllowed(orign => true)
			.AllowAnyMethod()
			.AllowAnyHeader()
			.AllowCredentials());

app.MapControllers();
app.MapHealthChecks("/health");

app.Run();