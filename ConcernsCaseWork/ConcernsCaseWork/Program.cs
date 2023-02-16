using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;


namespace ConcernsCaseWork
{
    public class Program
    {
        public static void Main(string[] args)
        {
            
            CreateHostBuilder(args).Build().Run();
        }

        private static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
	            .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder
	                    .ConfigureKestrel(options => options.AddServerHeader = false)
	                    .UseStartup<Startup>();
                });
    }
}
