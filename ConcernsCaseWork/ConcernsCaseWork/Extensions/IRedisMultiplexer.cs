using StackExchange.Redis;

namespace ConcernsCaseWork.Extensions
{
	/// <summary>
	/// Interface used in StartupExtension
	/// </summary>
	public interface IRedisMultiplexer
	{
		IConnectionMultiplexer Connect(ConfigurationOptions configuration);
	}
}