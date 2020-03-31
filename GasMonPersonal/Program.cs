using System;
using System.Threading;
using System.Threading.Tasks;
using GasMonPersonal.AWS;
using GasMonPersonal.GasListening;
using GasMonPersonal.Locations;
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

            await TestQueue();
            
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

            await new NotificationManager(apiClient).StartProcessingGasNotifications();
            
            Thread.Sleep(120_000);
        }
    }
}