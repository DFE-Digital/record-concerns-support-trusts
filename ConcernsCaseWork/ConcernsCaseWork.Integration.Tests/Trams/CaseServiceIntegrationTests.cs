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

		//[Test] TODO Enable when staging endpoint is deployed 
		public async Task WhenGetCasesByCaseworkerAndStatus_ReturnsListCaseDto()
		{
			// arrange
			var caseService = _factory.Services.GetRequiredService<ICaseService>();
			var trustSummaryDto = await FetchRandomTrust("Senior");
			
			var createCaseDto = CaseFactory.BuildCreateCaseDto(CaseWorker, trustSummaryDto.UkPrn);
			var caseDto = await PostCase(createCaseDto);
			
			// act
			var casesDto = await caseService.GetCasesByCaseworkerAndStatus(new CaseCaseWorkerSearch(createCaseDto.CreatedBy, createCaseDto.StatusUrn));

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

		[Test]
		public async Task WhenPatchCaseByUrn_UpdatesCache()
		{
			// arrange
			var caseService = _factory.Services.GetRequiredService<ICaseService>();
			var trustSummaryDto = await FetchRandomTrust("Senior");

			var createCaseDto = CaseFactory.BuildCreateCaseDto(CaseWorker, trustSummaryDto.UkPrn);
			var postCaseDto = await PostCase(createCaseDto);
			
			// Update case properties
			var timeNow = DateTimeOffset.Now;
			var toUpdateCaseDto = new CaseDto(
				postCaseDto.CreatedAt,
				timeNow,
				timeNow,
				timeNow,
				postCaseDto.CreatedBy,
				"some updated description",
				"some updated crm enquiry",
				postCaseDto.TrustUkPrn,
				"some updated reason at review",
				postCaseDto.DeEscalation,
				"some updated issue",
				postCaseDto.CurrentStatus,
				postCaseDto.NextSteps,
				"some updated case aim",
				postCaseDto.DeEscalationPoint,
				DirectionOfTravelEnum.Improving.ToString(),
				postCaseDto.Urn,
				postCaseDto.StatusUrn);
			
			// act
			var updatedCaseDto = await caseService.PatchCaseByUrn(toUpdateCaseDto);
			
			// assert
			Assert.That(updatedCaseDto, Is.Not.Null);
			Assert.That(updatedCaseDto.Description, Is.EqualTo("some updated description"));
			Assert.That(updatedCaseDto.Issue, Is.EqualTo("some updated issue"));
			Assert.That(updatedCaseDto.Urn, Is.EqualTo(postCaseDto.Urn));
			Assert.That(updatedCaseDto.CaseAim, Is.EqualTo("some updated case aim"));
			Assert.That(updatedCaseDto.ClosedAt, Is.EqualTo(timeNow));
			Assert.That(updatedCaseDto.CreatedAt, Is.EqualTo(postCaseDto.CreatedAt));
			Assert.That(updatedCaseDto.CreatedBy, Is.EqualTo(postCaseDto.CreatedBy));
			Assert.That(updatedCaseDto.CrmEnquiry, Is.EqualTo("some updated crm enquiry"));
			Assert.That(updatedCaseDto.CurrentStatus, Is.EqualTo(postCaseDto.CurrentStatus));
			Assert.That(updatedCaseDto.DeEscalation, Is.EqualTo(postCaseDto.DeEscalation));
			Assert.That(updatedCaseDto.NextSteps, Is.EqualTo(postCaseDto.NextSteps));
			Assert.That(updatedCaseDto.ReviewAt, Is.EqualTo(timeNow));
			Assert.That(updatedCaseDto.StatusUrn, Is.EqualTo(postCaseDto.StatusUrn));
			Assert.That(updatedCaseDto.UpdatedAt, Is.EqualTo(timeNow));
			Assert.That(updatedCaseDto.DeEscalationPoint, Is.EqualTo(postCaseDto.DeEscalationPoint));
			Assert.That(updatedCaseDto.DirectionOfTravel, Is.EqualTo(DirectionOfTravelEnum.Improving.ToString()));
			Assert.That(updatedCaseDto.ReasonAtReview, Is.EqualTo("some updated reason at review"));
			Assert.That(updatedCaseDto.TrustUkPrn, Is.EqualTo(postCaseDto.TrustUkPrn));
		}
		
		private async Task<CaseDto> PostCase(CreateCaseDto createCaseDto)
		{
			var caseService = _factory.Services.GetRequiredService<ICaseService>();
			return await caseService.PostCase(createCaseDto);
		}

		private async Task<TrustSummaryDto> FetchRandomTrust(string searchParameter)
		{
			var trustService = _factory.Services.GetRequiredService<ITrustService>();
			var apiWrapperTrusts= await trustService.GetTrustsByPagination(
				TrustFactory.BuildTrustSearch(searchParameter, searchParameter, searchParameter));
			
			Assert.That(apiWrapperTrusts, Is.Not.Null);
			Assert.That(apiWrapperTrusts.Data, Is.Not.Null);
			
			var random = new Random();
			int index = random.Next(apiWrapperTrusts.Data.Count);

			return apiWrapperTrusts.Data[index];
		}
	}
}