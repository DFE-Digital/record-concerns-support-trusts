using ConcernsCaseWork.Models;
using NUnit.Framework;
using System;
using System.Collections.Generic;

namespace ConcernsCaseWork.Tests.Models
{
	[Parallelizable(ParallelScope.All)]
	public class TrustCasesModelTests
	{
		[TestCase("live", "Open")]
		[TestCase("close", "Closed")]
		public void WhenBuildTrustCasesModel_ReturnsValidLogic(string actualStatus, string expectedStatus)
		{
			// arrange
			var createdAtDate = DateTimeOffset.Now;
			var closeDate = DateTimeOffset.Now;
			
			var trustCaseModel = new TrustCasesModel(
				1,
				"case-type",
				"case-sub-type",
				new Tuple<int, IList<string>>(1, new List<string> { "red" }),
				new List<string>{ "red" },
				createdAtDate, 
				closeDate, 
				actualStatus
			);
			
			// assert
			Assert.That(trustCaseModel.CaseUrn, Is.EqualTo(1));
			Assert.That(trustCaseModel.Closed, Is.EqualTo(closeDate.ToString("dd-MM-yyyy")));
			Assert.That(trustCaseModel.RagRating, Is.Not.Null);
			Assert.That(trustCaseModel.RagRating.Item1, Is.EqualTo(1));
			Assert.That(trustCaseModel.RagRating.Item2, Is.EqualTo(new List<string> { "red" }));
			Assert.That(trustCaseModel.StatusDescription, Is.EqualTo(expectedStatus));
			Assert.That(trustCaseModel.CaseTypeDescription, Is.EqualTo("case-type: case-sub-type"));
			Assert.That(trustCaseModel.RagRatingCss, Is.EqualTo(new List<string>{ "red" }));
		}
	}
}