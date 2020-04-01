using System.Threading.Tasks;
using GasMonPersonal.AWS;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace GasMonPersonal
{
    class Program
    {
        static async Task  Main(string[] args)
        {
            await GasMonApp.Run();
        }

        private static void CreateHostBuilder(string[] args)
        {
            Host.CreateDefaultBuilder()
                .ConfigureAppConfiguration(config => config.AddJsonFile("secrets.json"))
                .ConfigureServices(serviceCollection => serviceCollection.AddTransient<AwsApiClient>());
        }
    }
}