using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using AzQueueStorage;
using Azure.Storage.Queues;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace nsWeatherDataService
{
    public class WeatherDataService : BackgroundService
    {
        private readonly ILogger logger;
        private readonly QueueClient queueClient;

        public WeatherDataService(ILogger<WeatherDataService> _logger, QueueClient queueClient)
        {
            this.logger = _logger;
            this.queueClient = queueClient;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while(!stoppingToken.IsCancellationRequested)
            {
                var msg  = await queueClient.ReceiveMessageAsync();
                if(msg.Value != null){
                    var weatherMsg = JsonSerializer.Deserialize<WeatherForecast>(msg.Value.Body);
                    this.logger.LogInformation(weatherMsg.Summary);
                    await queueClient.DeleteMessageAsync(msg.Value.MessageId, msg.Value.PopReceipt);
                }
                await Task.Delay(System.TimeSpan.FromSeconds(10));
            }
            
        }
    }
}