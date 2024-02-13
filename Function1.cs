using System.Net;
using System.Text.Json;
using Azure;
using Azure.Messaging;
using Azure.Messaging.EventGrid;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;

namespace Functionproject
{
    public class Function1
    {
        private readonly ILogger _logger;
        private readonly string _topicKey;
        private readonly string _topicEndpoint;


        public Function1(ILoggerFactory loggerFactory)
        {
            _logger = loggerFactory.CreateLogger<Function1>();
            _topicEndpoint = "EduiAanxSDI1FPPChpb1owJYO+3Q8MsQd7xbKE2gLPM=";
            _topicKey = "Q3BL0g0iZdM5MQkt8JfssKyTFRygEiZVqBniyzkT+gc=";
        }

        [Function("F1")]
        public async Task<HttpResponseData> RunF1([HttpTrigger(AuthorizationLevel.Anonymous, "get", "post")] HttpRequestData req)
        {
            _logger.LogInformation("C# HTTP trigger function processed a request.");

            var response = req.CreateResponse(HttpStatusCode.OK);
            response.Headers.Add("Content-Type", "text/plain; charset=utf-8");

           await SendEventToTopicAsync();

            response.WriteString("The event was sent!");

            return response;
        }

        [Function("F2")]
        public void RunF2([EventGridTrigger] EventGridEvent cloudEvent)
        {
            _logger.LogInformation($"Received event data: {JsonSerializer.Serialize(cloudEvent.Data)}");

            cloudEvent.Data.ToString();
        }

        private async Task SendEventToTopicAsync()
        {
            var client = new EventGridPublisherClient(new Uri(_topicEndpoint), new AzureKeyCredential(_topicKey));

            var events = new List<EventGridEvent>
    {
        new EventGridEvent(
            subject: "/subscribers/user123/movies/357",
            dataVersion: "1.0",
            eventType: "MovieNotification",
            data: new BinaryData(JsonSerializer.Serialize(new
            {
                subscriberId = "user123",
                movieId = "357",
                movieTitle = "Shutter Island",
                releaseDate = "2024-03-10",
                notificationMessage = "Your favourite movie is coming to Netflix on 2024-03-10!"
            }))
        )
    };
            try
            {
                await client.SendEventsAsync(events);
                _logger.LogInformation("Event was sent successfully.");
            }
            catch (Exception ex)
            {
                _logger.LogError($"Failed to send the event. Exception: {ex.Message}");
            }
        }

    }
}


