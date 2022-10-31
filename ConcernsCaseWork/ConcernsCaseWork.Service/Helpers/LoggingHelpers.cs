using System.Runtime.CompilerServices;

namespace ConcernsCaseWork.Service.Helpers
{
	public static class LoggingHelpers
	{
		public static string EchoCallerName([CallerMemberName] string callerName = "")
		{
			return callerName;
		}
	}
}
