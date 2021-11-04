using ConcernsCaseWork.Integration.Tests.Factory;
using ConcernsCaseWork.Shared.Tests.Factory;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;
using Service.TRAMS.Cases;
using Service.TRAMS.Trusts;
using System;
using System.Threading.Tasks;

namespace ConcernsCaseWork.Integration.Tests.Trams
{
	[TestFixture]
	public class CaseServiceIntegrationTests
	{
		/// Testing the class requires a running Redis,
		/// startup is configured to use Redis with session storage.
		private IConfigurationRoot _configuration;
		private WebAppFactory _factory;

		/// Variables for caseworker and trustukprn, creates cases on Academies API.
		/// Future work can be to delete the records from the SQLServer.
		private const string CaseWorker = "case.service.integration";
		
		[OneTimeSetUp]
		public void OneTimeSetup()
		{
			_configuration = new ConfigurationBuilder().ConfigurationUserSecretsBuilder().Build();
			_factory = new WebAppFactory(_configuration);
		}
		
		[OneTimeTearDown]
		public void OneTimeTearDown()
		{
			_factory.Dispose();
		}

		[Test]
		public async Task WhenGetCasesByCaseworkerAndStatus_ReturnsListCaseDto()
		{
			// arrange
			var caseService = _factory.Services.GetRequiredService<ICaseService>();
			var trustSummaryDto = await FetchRandomTrust("Senior");
			
			var createCaseDto = CaseFactory.BuildCreateCaseDto(CaseWorker, trustSummaryDto.UkPrn);
			var caseDto = await PostCase(createCaseDto);
			
			// act
			var casesDto = await caseService.GetCasesByCaseworkerAndStatus(createCaseDto.CreatedBy, createCaseDto.StatusUrn);

			// assert
			Assert.That(caseDto, Is.Not.Null);
			Assert.That(casesDto, Is.Not.Null);
		}

		[Test]
		public async Task WhenGetCaseByUrn_ReturnsCaseDto()
		{
			// arrange
			var caseService = _factory.Services.GetRequiredService<ICaseService>();
			var trustSummaryDto = await FetchRandomTrust("Senior");

			var createCaseDto = CaseFactory.BuildCreateCaseDto(CaseWorker, trustSummaryDto.UkPrn);
			var postCaseDto = await PostCase(createCaseDto);
			
			// act
			var caseDto = await caseService.GetCaseByUrn(postCaseDto.Urn);

			// assert
			Assert.That(postCaseDto, Is.Not.Null);
			Assert.That(caseDto, Is.Not.Null);
		}
		
		[Test]
		public async Task WhenGetCasesByTrustUkPrn_ReturnsApiWrapperCaseDto()
		{
			// arrange
			var caseService = _factory.Services.GetRequiredService<ICaseService>();
			var trustSummaryDto = await FetchRandomTrust("Senior");

			var createCaseDto = CaseFactory.BuildCreateCaseDto(CaseWorker, trustSummaryDto.UkPrn);
			var postCaseDto = await PostCase(createCaseDto);
			
			// act
			var apiWrapperCaseDto = await caseService.GetCasesByTrustUkPrn(new CaseTrustSearch(postCaseDto.TrustUkPrn));

			// assert
			Assert.That(postCaseDto, Is.Not.Null);
			Assert.That(apiWrapperCaseDto, Is.Not.Null);
			Assert.That(apiWrapperCaseDto.Data, Is.Not.Null);
			Assert.That(apiWrapperCaseDto.Paging, Is.Not.Null);
		}
		
		private async Task<CaseDto> PostCase(CreateCaseDto createCaseDto)
		{
			var caseService = _factory.Services.GetRequiredService<ICaseService>();
			return await caseService.PostCase(createCaseDto);
		}

		private async Task<TrustSummaryDto> FetchRandomTrust(string searchParameter)
		{
			var trustService = _factory.Services.GetRequiredService<ITrustService>();
			var trusts= await trustService.GetTrustsByPagination(
				TrustFactory.BuildTrustSearch(searchParameter, searchParameter, searchParameter));
			
			var random = new Random();
			int index = random.Next(trusts.Count);

			return trusts[index];
		}
	}
}