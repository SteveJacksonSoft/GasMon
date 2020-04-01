using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using GasMonPersonal.AWS;
using GasMonPersonal.Locations;
using GasMonPersonal.MessageProcessing;
using GasMonPersonal.NotificationListening;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace GasMonPersonal
{
    class Program
    {
        static async Task  Main(string[] args)
        {
            CreateHostBuilder(args);

            await TestMessageProcessing();
            
            // await TestLocations();
        }

        private static void CreateHostBuilder(string[] args)
        {
            Host.CreateDefaultBuilder()
                .ConfigureAppConfiguration(config => config.AddJsonFile("secrets.json"))
                .ConfigureServices(serviceCollection => serviceCollection.AddTransient<AwsApiClient>());
        }

        private static async Task TestLocations()
        {
            var apiClient = new AwsApiClient();

            var locations = await LocationFetching.FetchLocations(apiClient);
            
            foreach (var location in locations)
            {
                Console.WriteLine($"{location.Id}: ({location.X}, {location.Y})");
            }
        }

        private static async Task TestQueue()
        {
            var apiClient = new AwsApiClient();

            await using var notificationManager = new NotificationManager(apiClient);
            await notificationManager.StartProcessingGasNotifications();
            
            Thread.Sleep(120_000);
        }

        private static async Task TestMessageProcessing()
        {
            var apiClient = new AwsApiClient();

            var locations = await LocationFetching.FetchLocations(apiClient);
            
            using var messageProcessor = new MessageProcessor(locations.ToList());

            await using var notificationManager = new NotificationManager(apiClient, messageProcessor.ProcessMessage);
            
            await notificationManager.StartProcessingGasNotifications();
            
            Thread.Sleep(20_000);
        }
    }
}