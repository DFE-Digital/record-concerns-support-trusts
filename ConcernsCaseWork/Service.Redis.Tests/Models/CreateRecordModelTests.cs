﻿using ConcernsCaseWork.Shared.Tests.Factory;
using NUnit.Framework;

namespace Service.Redis.Tests.Models
{
	[Parallelizable(ParallelScope.All)]
	public class CreateRecordModelTests
	{
		[Test]
		public void WhenCreateRecordModel_IsValid()
		{
			// arrange
			var createRecordModel = RecordFactory.BuildCreateRecordModel();
			
			// assert
			Assert.That(createRecordModel, Is.Not.Null);
			Assert.That(createRecordModel.TypeDisplay, Is.Not.Null);
			Assert.That(createRecordModel.Type, Is.Not.Null);
			Assert.That(createRecordModel.RagRating, Is.Not.Null);
			Assert.That(createRecordModel.RatingName, Is.Not.Null);
			Assert.That(createRecordModel.RatingUrn, Is.Not.Null);
			Assert.That(createRecordModel.SubType, Is.Not.Null);
			Assert.That(createRecordModel.TypeUrn, Is.Not.Null);
			Assert.That(createRecordModel.RagRatingCss, Is.Not.Null);
		}
	}
}