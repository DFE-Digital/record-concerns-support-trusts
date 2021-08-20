using StackExchange.Redis;

namespace ConcernsCaseWork.Extensions
{
	public sealed class RedisMultiplexer : IRedisMultiplexer
	{
		public IConnectionMultiplexer Connect(ConfigurationOptions configuration)
		{
			return ConnectionMultiplexer.Connect(configuration);
		}
	}
}