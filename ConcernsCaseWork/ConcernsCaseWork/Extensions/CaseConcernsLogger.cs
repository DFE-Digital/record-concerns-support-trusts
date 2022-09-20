using Microsoft.Extensions.Logging;
using System;
using System.Runtime.CompilerServices;

namespace ConcernsCaseWork.Extensions
{
	public static class LoggerExtensions
	{
		public static void LogMethodEntered<T>(this ILogger<T> logger, [CallerMemberName] string memberName = "", [CallerFilePath] string filePath = "", params object[] args)
		{
			logger.LogInformation($"{filePath}::{memberName} entered", args);
		}

		public static void LogDebugMsg<T>(this ILogger<T> logger, [CallerMemberName] string memberName = "", [CallerFilePath] string filePath = "", params object[] args)
		{
			logger.LogDebug($"{filePath}::{memberName} entered", args);
		}

		public static void LogInformationMsg<T>(this ILogger<T> logger, string message, [CallerMemberName] string memberName = "", [CallerFilePath] string filePath = "", params object[] args)
		{
			logger.LogInformation($"{filePath}::{memberName} {{Message}}", message, args);
		}

		public static void LogWarningMsg<T>(this ILogger<T> logger, string message, [CallerMemberName] string memberName = "", [CallerFilePath] string filePath = "", params object[] args)
		{
			logger.LogWarning($"{filePath}::{memberName} {{Message}}", message, args);
		}

		public static void LogErrorMsg<T>(this ILogger<T> logger, Exception ex, [CallerMemberName] string memberName = "", [CallerFilePath] string filePath = "", params object[] args)
		{
			logger.LogError($"{filePath}::{memberName}::Exception - {{Message}}", ex.Message, args);
		}
		public static void LogErrorMsg<T>(this ILogger<T> logger, string errorMessage, [CallerMemberName] string memberName = "", [CallerFilePath] string filePath = "", params object[] args)
		{
			logger.LogError($"{filePath}::{memberName}::Exception - {{Message}}", errorMessage, args);
		}
	}
}
