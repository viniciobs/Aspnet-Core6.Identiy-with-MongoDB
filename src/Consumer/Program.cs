using Consumer.Factories;
using Consumer.Handlers;
using Consumer.Models;
using Consumer.Templates;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Text;
using System.Web;
using LoggerFactory = Consumer.Factories.LoggerFactory;

using var consumer = ConsumerFactory.CreateConsumer();

var logger = LoggerFactory.CreateLogger();

var cancellation = new CancellationTokenSource();

Console.CancelKeyPress += (_, e) =>
{
    e.Cancel = true;
    cancellation.Cancel();
};

var template = TemplateLoader.LoadByName("EmailConfirmation.html");
var client = new HttpClient();

while (true)
{
    try
    {
        var result = consumer.Consume(cancellation.Token);

        logger.LogInformation(result.Message.Value);

        var emailData = JsonConvert.DeserializeObject<Email>(result.Message.Value);

        var body = template;
        body = body.Replace("###NAME###", emailData.UserName);
        body = body.Replace("###LINK###", $"https://localhost:7173/api/v1/confirm-email?userid={emailData.UserId}&token={HttpUtility.UrlEncode(emailData.ConfirmationToken)}");

        var json = JsonConvert.SerializeObject(
            new {
                To = emailData.Address,
                Subject = "Confirm your e-mail",
                IsHtml = true,
                Body = body,
            });

        var response = await client.PostAsync(
            requestUri: "https://localhost:7189/send",
            content: new StringContent(
                json,
                Encoding.UTF8,
                "application/json"));

        if (response.IsSuccessStatusCode is false)
        {
            logger.LogError(JsonConvert.SerializeObject(response));
        }
    }
    catch (Exception exception)
    {
        exception.Handle(logger);
    }
}