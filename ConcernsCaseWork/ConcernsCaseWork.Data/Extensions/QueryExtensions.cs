using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConcernsCaseWork.Data.Extensions
{
	public static class QueryExtensions
	{
		public static IQueryable<T> Paginate<T>(this IQueryable<T> query, int page, int count)
		{
			return query.Skip((page - 1) * count).Take(count);
		}
	}
}
