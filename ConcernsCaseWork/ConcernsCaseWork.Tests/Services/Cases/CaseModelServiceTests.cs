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
					Assert.That(expected.TrustName, Is.EqualTo(actual.TrustName));
					Assert.That(expected.Rag, Is.EqualTo(actual.Rag));
					Assert.That(expected.Type, Is.EqualTo(actual.Type));
					Assert.That(expected.Created, Is.EqualTo(actual.Created));
					Assert.That(expected.LastUpdate, Is.EqualTo(actual.LastUpdate));
					Assert.That(expected.Closed, Is.EqualTo(actual.Closed));
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