using Microsoft.EntityFrameworkCore.Diagnostics;
using System.Data.Common;

namespace ConcernsCaseWork.Data.EFInterceptors
{
	/// <summary>
	/// Logs translated SQL commands
	/// </summary>
	public class SqlCommandInterceptor : DbCommandInterceptor
	{
		public override InterceptionResult<DbDataReader> ReaderExecuting(
			DbCommand command,
			CommandEventData eventData,
			InterceptionResult<DbDataReader> result)
		{
			Console.WriteLine($"Executing command: {command.CommandText}");
			return base.ReaderExecuting(command, eventData, result);
		}
	}
}
