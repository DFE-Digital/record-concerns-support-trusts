using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Service.TRAMS.Helpers
{
	public static class LoggingHelpers
	{
		public static string EchoCallerName([CallerMemberName] string callerName = "")
		{
			return callerName;
		}
	}
}
