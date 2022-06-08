using ConcernsCaseWork.Constraints;
using Microsoft.AspNetCore.Routing;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace ConcernsCaseWork.Tests.Routing
{
	[Parallelizable(ParallelScope.All)]
	public class FinancialPlanEditModeConstraintTests
	{
		[Test]	
		[TestCase("Edit", true)]
		[TestCase("edit", true)]
		[TestCase("close", true)]
		[TestCase("Close", true)]
		[TestCase("create", false)]
		[TestCase("update", false)]
		[TestCase("blah", false)]
		[TestCase("foo", false)]
		public void FinancialPlanEditModeConstraint_Match_Only_Allowed_Actions(string routeValue, bool expectedMatch)
		{
			// arrange
			var sut = new FinancialPlanEditModeConstraint();
			var routeValues = new RouteValueDictionary();
			routeValues.Add("editMode", routeValue);

			// act 
			bool actualResult = sut.Match(null, null, "editMode", routeValues, RouteDirection.IncomingRequest);

			// assert
			Assert.AreEqual(expectedMatch, actualResult);
		}
	}
}
