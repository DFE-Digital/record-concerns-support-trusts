using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace ConcernsCaseWork.Data.Tests.Helpers
{
	public static class TestExtensions
	{
		public static string ToKpiDateFormat(this DateTime date)
		{
			return date.ToString("dd-MM-yyyy");
		}
	}
}
