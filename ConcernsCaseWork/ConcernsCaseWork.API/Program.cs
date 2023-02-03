namespace ConcernsCaseWork.API
{
	public class Program
	{
		/// <summary>
		/// This is a bit misleading, the api doesn't actually host itself (though it probably should), so this method will never be invoked
		/// </summary>
		/// <param name="args"></param>

		public static void Main(string[] args)
		{
			//CreateHostBuilder(args).Build().Run();
		}

		//public static IHostBuilder CreateHostBuilder(string[] args) =>
		//	Host.CreateDefaultBuilder(args)
		//		.ConfigureLogging(c => {
		//			c.ClearProviders();
		//			c.AddConsole();
		//		})
		//		.ConfigureWebHostDefaults(webBuilder => { webBuilder.UseStartup<Startup>(); });
	}
}
