using AutoMapper;
using ConcernsCaseWork.Mappers;
using ConcernsCaseWork.Models;
using ConcernsCaseWork.Services.Cases;
using ConcernsCaseWork.Shared.Tests.Factory;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using Service.TRAMS.Cases;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ConcernsCaseWork.Tests.Services.Cases
{
	[Parallelizable(ParallelScope.All)]
	public class CaseModelServiceTests
	{
		[Test]
		public async Task WhenGetCasesByCaseworker_ReturnsCases()
		{
			// arrange
			var mockCaseService = new Mock<ICaseService>();
			var mockLogger = new Mock<ILogger<CaseModelService>>();
			var config = new MapperConfiguration(cfg => cfg.AddProfile<AutoMapping>());
			var mapper = config.CreateMapper();
			var casesDto = CaseDtoFactory.CreateListCaseDto();

			mockCaseService.Setup(cs => cs.GetCasesByCaseworker(It.IsAny<string>())).ReturnsAsync(casesDto);

			// act
			var caseModelService = new CaseModelService(mockCaseService.Object, mapper, mockLogger.Object);
			var casesModel = await caseModelService.GetCasesByCaseworker(It.IsAny<string>());

			// assert
			Assert.IsAssignableFrom<List<CaseModel>>(casesModel);
			Assert.That(casesModel.Count, Is.EqualTo(casesDto.Count));
			foreach (var expected in casesModel)
			{
				foreach (var actual in casesDto.Where(actual => expected.Id.Equals(actual.Id)))
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
					Assert.That(expected.UpdateAt, Is.EqualTo(actual.UpdateAt));
					Assert.That(expected.DirectionOfTravel, Is.EqualTo(actual.DirectionOfTravel));
					Assert.That(expected.ReasonAtReview, Is.EqualTo(actual.ReasonAtReview));
					Assert.That(expected.TrustUkPrn, Is.EqualTo(actual.TrustUkPrn));
				}
			}
		}
		
		[Test]
		public async Task WhenGetCasesByCaseworker_ThrowsException_ReturnEmptyCases()
		{
			// arrange
			var mockCaseService = new Mock<ICaseService>();
			var mockLogger = new Mock<ILogger<CaseModelService>>();
			var config = new MapperConfiguration(cfg => cfg.AddProfile<AutoMapping>());
			var mapper = config.CreateMapper();

			mockCaseService.Setup(cs => cs.GetCasesByCaseworker(It.IsAny<string>())).ThrowsAsync(new Exception());

			// act
			var caseModelService = new CaseModelService(mockCaseService.Object, mapper, mockLogger.Object);
			var casesModel = await caseModelService.GetCasesByCaseworker(It.IsAny<string>());

			// assert
			Assert.IsAssignableFrom<List<CaseModel>>(casesModel.ToList());
			Assert.That(casesModel.Count, Is.EqualTo(0));
		}
	}
}