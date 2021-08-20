using Microsoft.Extensions.Logging;

namespace ConcernsCaseWork.Tests.Factory
{
	internal static class LogFactory
	{
		public static ILogger<T> CreateLog<T>()
		{
			using var loggerFactory = LoggerFactory.Create(builder => builder.AddConsole());
			return loggerFactory.CreateLogger<T>();
		}

		public static ILogger CreateLog(string categoryName)
		{
			using var loggerFactory = LoggerFactory.Create(builder => builder.AddConsole());
			return loggerFactory.CreateLogger(categoryName);
		}
	}
}