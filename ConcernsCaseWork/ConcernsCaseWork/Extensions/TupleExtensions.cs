using System;
using System.Collections.Generic;
using System.Linq;

namespace ConcernsCaseWork.Extensions
{
	public static class TupleExtensions
	{
		public static (IList<T>, IList<T>) Split<T>(this IEnumerable<T> source, Func<T, bool> statusLive, Func<T, bool> statusMonitoring)
		{
			IEnumerable<T> enumerable = source.ToList();
			var activeCases = enumerable.Where(statusLive).ToList();
			var monitoringCases = enumerable.Where(statusMonitoring).ToList();

			return (activeCases, monitoringCases);
		}
	}
}