using AutoMapper;
using ConcernsCaseWork.Mappers;
using ConcernsCaseWork.Models;
using ConcernsCaseWork.Services.Cases;
using ConcernsCaseWork.Tests.Factory;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using Service.TRAMS.Cases;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ConcernsCaseWork.Tests.Services.Cases
{
	[Parallelizable(ParallelScope.All)]
	public class CaseModelServiceTests
	{
		[Test]
		public async Task WhenGetCasesByCaseworkerReturnsCases()
		{
			// arrange
			var mockCaseService = new Mock<ICaseService>();
			var mockLogger = new Mock<ILogger<CaseModelService>>();
			var config = new MapperConfiguration(cfg => cfg.AddProfile<AutoMapping>());
			var mapper = config.CreateMapper();
			var casesDto = CaseDtoFactory.CreateCaseModels();

			mockCaseService.Setup(cs => cs.GetCasesByCaseworker(It.IsAny<string>())).ReturnsAsync(casesDto);

			// act
			var caseModelService = new CaseModelService(mockLogger.Object, mockCaseService.Object, mapper);
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
					Assert.That(expected.DaysOpen, Is.EqualTo(actual.DaysOpen));
				}
			}
		}
	}
}