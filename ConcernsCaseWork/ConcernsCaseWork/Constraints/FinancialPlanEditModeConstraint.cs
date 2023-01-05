using ConcernsCaseWork.Enums;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using System;
using System.Linq;

namespace ConcernsCaseWork.Constraints
{
	public class FinancialPlanEditModeConstraint : IRouteConstraint
	{
		private static readonly string[] _editModes = Enum.GetValues(typeof(FinancialPlanEditMode)).Cast<FinancialPlanEditMode>()
												.Select(em => em.ToString()).ToArray();

		public bool Match(HttpContext httpContext, IRouter route, string routeKey, RouteValueDictionary values, RouteDirection routeDirection)
		{
			return _editModes.Contains(values[routeKey]?.ToString(), StringComparer.OrdinalIgnoreCase);
		}
	}
}
