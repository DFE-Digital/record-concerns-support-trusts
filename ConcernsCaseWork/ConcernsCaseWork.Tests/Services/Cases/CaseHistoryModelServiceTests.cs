using AutoMapper;
using ConcernsCaseWork.Extensions;
using ConcernsCaseWork.Mappers;
using ConcernsCaseWork.Models;
using ConcernsCaseWork.Redis.Cases;
using ConcernsCaseWork.Services.Cases;
using ConcernsCaseWork.Shared.Tests.Factory;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using ConcernsCaseWork.Service.Cases;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ConcernsCaseWork.Tests.Services.Cases
{
	[Parallelizable(ParallelScope.All)]
	public class CaseHistoryModelServiceTests
	{
		[Test]
		public async Task WhenGetCasesHistory_ReturnsCasesHistoryModel()
		{
			// arrange
			var mockCaseHistoryCachedService = new Mock<ICaseHistoryCachedService>();
			var mockLogger = new Mock<ILogger<CaseHistoryModelService>>();
			var config = new MapperConfiguration(cfg => cfg.AddProfile<AutoMapping>());
			var mapper = config.CreateMapper();

			var casesHistoryDto = CaseFactory.BuildListCasesHistoryDto();
			
			mockCaseHistoryCachedService.Setup(c =>
					c.GetCasesHistory(It.IsAny<CaseSearch>(), It.IsAny<string>()))
				.ReturnsAsync(casesHistoryDto);

			// act
			var caseHistoryModelService = new CaseHistoryModelService(mockCaseHistoryCachedService.Object, mapper, mockLogger.Object);
			var casesHistoryModel = await caseHistoryModelService.GetCasesHistory(It.IsAny<string>(), It.IsAny<long>());

			// assert
			Assert.IsAssignableFrom<List<CaseHistoryModel>>(casesHistoryModel);
			Assert.That(casesHistoryModel.Count, Is.EqualTo(casesHistoryDto.Count));
			
			foreach (var expected in casesHistoryModel)
			{
				foreach (var actual in casesHistoryDto.Where(actual => expected.CaseUrn.CompareTo(actual.CaseUrn) == 0))
				{
					Assert.That(expected.Action, Is.EqualTo(actual.Action));
					Assert.That(expected.Description, Is.EqualTo(actual.Description));
					Assert.That(expected.Title, Is.EqualTo(actual.Title));
					Assert.That(expected.Urn, Is.EqualTo(actual.Urn));
					Assert.That(expected.CreatedAt, Is.EqualTo(actual.CreatedAt));
					Assert.That(expected.CaseUrn, Is.EqualTo(actual.CaseUrn));
					Assert.That(expected.Created, Is.EqualTo(actual.CreatedAt.ToUserFriendlyDate()));
				}
			}
		}
	}
}