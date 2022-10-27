using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using Serilog;
using Serilog.Events;
using Serilog.Formatting.Compact;

namespace ConcernsCaseWork
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var logConfig = new LoggerConfiguration()
                .MinimumLevel.Override("Microsoft", LogEventLevel.Information)
                .Enrich.FromLogContext()
                .WriteTo.Console(new RenderedCompactJsonFormatter())
				.WriteTo.Sentry();

	        Log.Logger = logConfig.CreateLogger();

            Log.Information("Starting web host");
            CreateHostBuilder(args).Build().Run();
        }

        private static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .UseSerilog()
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder
	                    .UseSentry()
	                    .ConfigureKestrel(options => options.AddServerHeader = false)
	                    .UseStartup<Startup>();
                });
    }
}
