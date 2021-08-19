using NUnit.Framework;
using StackExchange.Redis;

namespace Service.Redis.Tests
{
	public class RedisConnectTests
	{
		[SetUp]
		public void Setup()
		{
		}

		[Test]
		public void RedisConnectSuccessTest()
		{
			/*var server = "master.cf-a7ls4an6embbm.b5xrdh.euw2.cache.amazonaws.com";
			var port = "6379";
			
			ConnectionMultiplexer redis = ConnectionMultiplexer.Connect(
				new ConfigurationOptions {
					Password = "iIssk9ZxDWIsiHifgMCoyZZzByblP4Mj",
					EndPoints = { $"{server}:{port}" },
					Ssl = false
				});
			
			var cache = redis.GetDatabase();

			string expectedStringData = "Hello world";
			cache.SetAdd("key003", System.Text.Encoding.UTF8.GetBytes(expectedStringData));
			var dataFromCache = cache.StringGet("key003");
			var actualCachedStringData = System.Text.Encoding.UTF8.GetString(dataFromCache);
			Assert.AreEqual(expectedStringData, actualCachedStringData);*/
		}
	}
}