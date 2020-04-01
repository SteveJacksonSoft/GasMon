using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
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
                .ConfigureAppConfiguration(config => config.AddJsonFile("secrets.json"));
        }
    }
}