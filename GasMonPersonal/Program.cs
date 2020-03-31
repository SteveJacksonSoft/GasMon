using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using GasMonPersonal.AWS;
using GasMonPersonal.Locations;
using GasMonPersonal.Models;
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

            await Test();
        }

        private static void CreateHostBuilder(string[] args)
        {
            Host.CreateDefaultBuilder()
                .ConfigureAppConfiguration(config => config.AddJsonFile("secrets.json"))
                .ConfigureServices(serviceCollection => serviceCollection.AddTransient<AwsApiClient>());
        }

        private static async Task Test()
        {
            var apiClient = new AwsApiClient();

            var locations = await LocationFetching.FetchLocations(apiClient);
            
            foreach (var location in locations)
            {
                Console.WriteLine($"{location.Id}: ({location.X}, {location.Y})");
            }
        }
    }
}