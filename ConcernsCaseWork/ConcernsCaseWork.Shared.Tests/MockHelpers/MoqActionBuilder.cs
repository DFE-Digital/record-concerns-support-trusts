using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Linq.Expressions;

namespace ConcernsCaseWork.Shared.Tests.MockHelpers;

// Taken from https://adamstorr.azurewebsites.net/blog/mocking-ilogger-with-moq

public static class MoqActionBuilder
{
	public static Mock<ILogger<T>> VerifyLogErrorWasCalled<T>(this Mock<ILogger<T>> logger, string expectedMessage)
		=> VerifyLogError(logger, expectedMessage, Times.Once());

	private static Mock<ILogger<T>> VerifyLogError<T>(this Mock<ILogger<T>> logger, string expectedMessage, Times timesCalled)
	{
		Func<object, Type, bool> state = (v, t) => v.ToString()?.CompareTo(expectedMessage) == 0;
    
		logger.Verify(
			x => x.Log(
				It.Is<LogLevel>(l => l == LogLevel.Error),
				It.IsAny<EventId>(),
				It.Is<It.IsAnyType>((v, t) => state(v, t)),
				It.IsAny<Exception>(),
				It.Is<Func<It.IsAnyType, Exception, string>>((v, t) => true)), timesCalled);

		return logger;
	}
	
	public static Mock<ILogger<T>> VerifyLogErrorWasNotCalled<T>(this Mock<ILogger<T>> logger)
	{
		logger.Verify(
			x => x.Log(
				It.Is<LogLevel>(l => l == LogLevel.Error),
				It.IsAny<EventId>(),
				It.Is<It.IsAnyType>((v, t) => true),
				It.IsAny<Exception>(),
				It.Is<Func<It.IsAnyType, Exception, string>>((v, t) => true)), Times.Never);

		return logger;
	}
}