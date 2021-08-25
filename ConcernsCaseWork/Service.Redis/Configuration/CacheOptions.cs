namespace Service.Redis.Configuration
{
	public class CacheOptions
	{
		public const string Cache = "Cache";

		public int TimeToLive { get; set; }
	}
}