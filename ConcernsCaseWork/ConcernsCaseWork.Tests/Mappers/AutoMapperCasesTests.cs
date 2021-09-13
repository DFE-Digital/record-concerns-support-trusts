using AutoMapper;
using ConcernsCaseWork.Mappers;
using ConcernsCaseWork.Models;
using ConcernsCaseWork.Shared.Tests.Factory;
using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;

namespace ConcernsCaseWork.Tests.Mappers
{
	[Parallelizable(ParallelScope.All)]
	public class AutoMapperCasesTests
	{
		[Test]
		public void ConfigurationIsValid()
		{
			// arrange
			var config = new MapperConfiguration(cfg => cfg.AddProfile<AutoMapping>());
			
			// assert
			config.AssertConfigurationIsValid();
		}

		[Test]
		public void ConvertFromCasesDtoToCasesModelIsValid()
		{
			// arrange
			var config = new MapperConfiguration(cfg => cfg.AddProfile<AutoMapping>());
			var mapper = config.CreateMapper();
			
			var casesDto = CaseDtoFactory.BuildListCaseDto();
			
			// act
			var casesModel = mapper.Map<IList<CaseModel>>(casesDto);
			
			// assert
			Assert.IsAssignableFrom<List<CaseModel>>(casesModel);
			Assert.That(casesModel.Count, Is.EqualTo(casesDto.Count));
			foreach (var expected in casesModel)
			{
				foreach (var actual in casesDto.Where(actual => expected.Urn.Equals(actual.Urn)))
				{
					Assert.That(expected.Description, Is.EqualTo(actual.Description));
					Assert.That(expected.Issue, Is.EqualTo(actual.Issue));
					Assert.That(expected.Status, Is.EqualTo(actual.Status));
					Assert.That(expected.Urn, Is.EqualTo(actual.Urn));
					Assert.That(expected.ClosedAt, Is.EqualTo(actual.ClosedAt));
					Assert.That(expected.CreatedAt, Is.EqualTo(actual.CreatedAt));
					Assert.That(expected.CreatedBy, Is.EqualTo(actual.CreatedBy));
					Assert.That(expected.CrmEnquiry, Is.EqualTo(actual.CrmEnquiry));
					Assert.That(expected.CurrentStatus, Is.EqualTo(actual.CurrentStatus));
					Assert.That(expected.DeEscalation, Is.EqualTo(actual.DeEscalation));
					Assert.That(expected.NextSteps, Is.EqualTo(actual.NextSteps));
					Assert.That(expected.ResolutionStrategy, Is.EqualTo(actual.ResolutionStrategy));
					Assert.That(expected.ReviewAt, Is.EqualTo(actual.ReviewAt));
					Assert.That(expected.UpdatedAt, Is.EqualTo(actual.UpdatedAt));
					Assert.That(expected.DirectionOfTravel, Is.EqualTo(actual.DirectionOfTravel));
					Assert.That(expected.ReasonAtReview, Is.EqualTo(actual.ReasonAtReview));
					Assert.That(expected.TrustUkPrn, Is.EqualTo(actual.TrustUkPrn));
				}
			}
		}
	}
}