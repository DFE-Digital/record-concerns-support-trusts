using Microsoft.Extensions.Logging;
using Moq;
using System;

namespace ConcernsCaseWork.Tests
{
	/// <summary>
	/// A collection of useful helper methods to make writing tests easier.
	/// If adding a new helper, ideally make it stateless.
	/// </summary>
	public class TestHelpers
	{
		public static void VerifyMethodEntryLogged<T>(Mock<ILogger<T>> mockLogger, string methodName, int times = 1)
		{
			mockLogger.Verify(
				m => m.Log(
					LogLevel.Information,
					It.IsAny<EventId>(),
					It.Is<It.IsAnyType>((v, _) => v.ToString().Contains($"::{methodName} entered")),
					null,
					It.IsAny<Func<It.IsAnyType, Exception, string>>()),
				Times.Exactly(times));
		}
	}
}
