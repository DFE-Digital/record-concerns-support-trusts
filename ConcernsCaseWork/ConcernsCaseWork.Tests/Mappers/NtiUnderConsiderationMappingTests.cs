using ConcernsCaseWork.Mappers;
using ConcernsCaseWork.Models.CaseActions;
using ConcernsCaseWork.Shared.Tests.Factory;
using NUnit.Framework;
using Service.TRAMS.NtiUnderConsideration;
using Service.TRAMS.Trusts;
using System;
using System.Linq;

namespace ConcernsCaseWork.Tests.Mappers
{
	[Parallelizable(ParallelScope.All)]
	public class NtiUnderConsiderationMappingTests
	{
		[Test]
		public void WhenMapDtoToServiceModel_ReturnsCorrectModel()
		{
			//arrange
			var ntiDto = new NtiUnderConsiderationDto
			{
				Id = 1L,
				CaseUrn = 123L,
				CreatedAt = DateTime.Now.AddDays(-5),
				ClosedAt = DateTime.Now,
				ClosedStatusId = 1,
				Notes = "Test notes",
				Reasons = new NtiUnderConsiderationReasonDto[]
				{
					new NtiUnderConsiderationReasonDto { Id = 11, Name = "Test reason 1"  },
					new NtiUnderConsiderationReasonDto { Id = 21, Name = "Test reason 2"  }
				},
				UnderConsiderationReasonsMapping = new int[] { 11, 21 },
				UpdatedAt = DateTime.Now.AddDays(-5)
			};


			// act
			var serviceModel = NtiUnderConsiderationMappers.ToServiceModel(ntiDto);

			// assert
			Assert.That(serviceModel, Is.Not.Null);
			Assert.That(serviceModel.CaseUrn, Is.EqualTo(ntiDto.CaseUrn));
			Assert.That(serviceModel.Id, Is.EqualTo(ntiDto.Id));
			Assert.That(serviceModel.NtiReasonsForConsidering, Is.Not.Null);
			Assert.That(serviceModel.NtiReasonsForConsidering.Count, Is.EqualTo(ntiDto.Reasons.Count));
			Assert.That(serviceModel.NtiReasonsForConsidering.ElementAt(0).Id, Is.EqualTo(ntiDto.Reasons.ElementAt(0).Id));
			Assert.That(serviceModel.NtiReasonsForConsidering.ElementAt(1).Id, Is.EqualTo(ntiDto.Reasons.ElementAt(1).Id));
		}

		[Test]
		public void WhenMapDtoToDbModel_ReturnsCorrectModel()
		{
			//arrange
			var serviceModel = new NtiUnderConsiderationModel
			{
				Id = 1L,
				CaseUrn = 123L,
				CreatedAt = DateTime.Now.AddDays(-5),
				ClosedAt = DateTime.Now,
				Notes = "Test notes",
				NtiReasonsForConsidering = new NtiReasonForConsideringModel[] {
					new NtiReasonForConsideringModel { Id = 11, Name = "Test reason 1"  },
					new NtiReasonForConsideringModel { Id = 21, Name = "Test reason 2"  }
				}
			};


			// act
			var dbModel = NtiUnderConsiderationMappers.ToDBModel(serviceModel);

			// assert
			Assert.That(dbModel, Is.Not.Null);
			Assert.That(dbModel.CaseUrn, Is.EqualTo(serviceModel.CaseUrn));
			Assert.That(dbModel.Id, Is.EqualTo(serviceModel.Id));

			Assert.That(dbModel.UnderConsiderationReasonsMapping, Is.Not.Null);
			Assert.That(dbModel.UnderConsiderationReasonsMapping.Count, Is.EqualTo(serviceModel.NtiReasonsForConsidering.Count));
			Assert.That(dbModel.UnderConsiderationReasonsMapping.ElementAt(0), Is.EqualTo(serviceModel.NtiReasonsForConsidering.ElementAt(0).Id));
			Assert.That(dbModel.UnderConsiderationReasonsMapping.ElementAt(1), Is.EqualTo(serviceModel.NtiReasonsForConsidering.ElementAt(1).Id));

			Assert.That(dbModel.Reasons, Is.Not.Null);
			Assert.That(dbModel.Reasons.Count, Is.EqualTo(serviceModel.NtiReasonsForConsidering.Count));
			Assert.That(dbModel.Reasons.ElementAt(0).Id, Is.EqualTo(serviceModel.NtiReasonsForConsidering.ElementAt(0).Id));
			Assert.That(dbModel.Reasons.ElementAt(1).Id, Is.EqualTo(serviceModel.NtiReasonsForConsidering.ElementAt(1).Id));

		}


	}
}