using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace WebService
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var loggerFactory = LoggerFactory.Create(builder => { builder.AddConsole(); });

            var host = Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(builder =>
                {
                    builder.UseStartup(context => new Startup(loggerFactory));
                })
                .Build();

            await host.RunAsync();
        }
    }
}
