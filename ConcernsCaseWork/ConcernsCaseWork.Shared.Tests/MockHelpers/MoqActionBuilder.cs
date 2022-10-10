using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Linq.Expressions;

namespace ConcernsCaseWork.Shared.Tests.MockHelpers;

// Taken from https://adamstorr.azurewebsites.net/blog/mocking-ilogger-with-moq

public static class MoqActionBuilder
{
	public static Mock<ILogger<T>> VerifyLogErrorWasCalled<T>(this Mock<ILogger<T>> logger, string expectedMessage = "")
		=> VerifyLogError(logger, expectedMessage, Times.Once());
	
	public static Mock<ILogger<T>> VerifyLogErrorWasNotCalled<T>(this Mock<ILogger<T>> logger, string expectedMessage = "")
		=> VerifyLogError(logger, expectedMessage, Times.Never());
	
	public static Mock<ILogger<T>> VerifyLogInformationWasCalled<T>(this Mock<ILogger<T>> logger, string expectedMessage = "")
		=> VerifyLogInformation(logger, expectedMessage, Times.Once());

	private static Mock<ILogger<T>> VerifyLogError<T>(this Mock<ILogger<T>> logger, string expectedMessage, Times timesCalled)
		=> VerifyLog(logger, expectedMessage, timesCalled, LogLevel.Error);
	
	private static Mock<ILogger<T>> VerifyLogInformation<T>(this Mock<ILogger<T>> logger, string expectedMessage, Times timesCalled)
		=> VerifyLog(logger, expectedMessage, timesCalled, LogLevel.Information);

	private static Mock<ILogger<T>> VerifyLog<T>(this Mock<ILogger<T>> logger, string expectedMessage, Times timesCalled, LogLevel logLevel)
	{
		Func<object, Type, bool> state = 
			(v, t) => 
				   string.IsNullOrEmpty(expectedMessage) 
				|| (v != null && v.ToString()!.Contains(expectedMessage));

		logger.Verify(
			x => x.Log(
				It.Is<LogLevel>(l => l == logLevel),
				It.IsAny<EventId>(),
				It.Is<It.IsAnyType>((v, t) => state(v, t)),
				It.IsAny<Exception>(),
				It.Is<Func<It.IsAnyType, Exception, string>>((v, t) => true)), timesCalled);

		return logger;
	}

}