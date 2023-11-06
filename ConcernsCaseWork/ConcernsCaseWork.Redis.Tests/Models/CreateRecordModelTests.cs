﻿using ConcernsCaseWork.Shared.Tests.Factory;
using NUnit.Framework;

namespace ConcernsCaseWork.Redis.Tests.Models
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
			Assert.That(createRecordModel.RatingId, Is.Not.Zero);
			Assert.That(createRecordModel.SubType, Is.Not.Zero);
			Assert.That(createRecordModel.TypeId, Is.Not.Zero);
		}

		[Test]
		public void WhenCreateRecordModel_SubTypeIsNull_ReturnsValidTypeDisplay()
		{
			// arrange
			var createRecordModel = RecordFactory.BuildCreateRecordModel();
			createRecordModel.SubType = null;

			// assert
			Assert.That(createRecordModel, Is.Not.Null);
			Assert.That(createRecordModel.Type, Is.Not.Null);
			Assert.That(createRecordModel.TypeDisplay, Is.Not.Null);
			var separator = string.IsNullOrEmpty(createRecordModel.SubType) ? string.Empty : ":";
			var expectedTypeDisplay = $"{createRecordModel.Type}{separator} {createRecordModel.SubType ?? string.Empty}";
			Assert.That(createRecordModel.TypeDisplay, Is.EqualTo(expectedTypeDisplay));
		}
	}
}