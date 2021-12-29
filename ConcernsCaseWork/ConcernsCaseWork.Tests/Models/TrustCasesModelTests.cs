using ConcernsCaseWork.Models;
using ConcernsCaseWork.Shared.Tests.Factory;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ConcernsCaseWork.Tests.Models
{
	[Parallelizable(ParallelScope.All)]
	public class TrustCasesModelTests
	{
		[Test]
		public void WhenBuildTrustCasesModel_ReturnsValidLogic()
		{
			// arrange
			var createdAtDate = DateTimeOffset.Now;
			var ratingModel = RatingFactory.BuildRatingModel();
			var recordsModel = RecordFactory.BuildListRecordModel();
			
			var trustCaseModel = new TrustCasesModel(
				1,
				recordsModel,
				ratingModel,
				createdAtDate
			);
			
			// assert
			Assert.That(trustCaseModel.CaseUrn, Is.EqualTo(1));
			Assert.That(trustCaseModel.RecordsModel, Is.Not.Null);
			Assert.That(trustCaseModel.RecordsModel.Count, Is.EqualTo(1));
			Assert.That(trustCaseModel.Created, Is.EqualTo(createdAtDate.ToString("dd-MM-yyyy")));
			Assert.That(trustCaseModel.RatingModel, Is.Not.Null);
			Assert.That(trustCaseModel.RatingModel.RagRating.Item1, Is.EqualTo(1));
			Assert.That(trustCaseModel.RatingModel.RagRating.Item2, Is.EqualTo(new List<string> { "red" }));
			Assert.That(trustCaseModel.RatingModel.RagRatingCss, Is.EqualTo(new List<string> { "red" }));
		}
	}
}