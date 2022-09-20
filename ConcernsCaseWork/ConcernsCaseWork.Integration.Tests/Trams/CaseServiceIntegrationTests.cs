using ConcernsCaseWork.Integration.Tests.Factory;
using ConcernsCasework.Service.Cases;
using ConcernsCasework.Service.Trusts;
using ConcernsCaseWork.Shared.Tests.Factory;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;
using System;
using System.Threading.Tasks;

namespace ConcernsCaseWork.Integration.Tests.Trams
{
	[TestFixture]
	public class CaseServiceIntegrationTests
	{
		/// <summary>
		/// Testing the class requires a running Redis,
		/// startup is configured to use Redis with session storage.
		/// </summary>
		private IConfigurationRoot _configuration;
		private WebAppFactory _factory;

		/// <summary>
		/// Variables for caseworker and trustukprn, creates cases on Academies API.
		/// Future work can be to delete the records from the SQLServer.
		/// </summary>
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
			using var serviceScope = _factory.Services.CreateScope();
			var caseService = serviceScope.ServiceProvider.GetRequiredService<ICaseService>();
			var trustSummaryDto = await FetchRandomTrust(serviceScope, "Senior");

			var createCaseDto = CaseFactory.BuildCreateCaseDto(CaseWorker, trustSummaryDto.UkPrn);
			var caseDto = await PostCase(serviceScope, createCaseDto);

			// act
			var casesDto = await caseService.GetCasesByCaseworkerAndStatus(new CaseCaseWorkerSearch( createCaseDto.CreatedBy, createCaseDto.StatusUrn));

			// assert
			Assert.That(caseDto, Is.Not.Null);
			Assert.That(casesDto, Is.Not.Null);
		}

		[Test]
		public async Task WhenGetCaseByUrn_ReturnsCaseDto()
		{
			// arrange
			using var serviceScope = _factory.Services.CreateScope();
			var caseService = serviceScope.ServiceProvider.GetRequiredService<ICaseService>();
			var trustSummaryDto = await FetchRandomTrust(serviceScope, "Senior");

			var createCaseDto = CaseFactory.BuildCreateCaseDto(CaseWorker, trustSummaryDto.UkPrn);
			var postCaseDto = await PostCase(serviceScope, createCaseDto);

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
			using var serviceScope = _factory.Services.CreateScope();
			var caseService = serviceScope.ServiceProvider.GetRequiredService<ICaseService>();
			var trustSummaryDto = await FetchRandomTrust(serviceScope, "Senior");

			var createCaseDto = CaseFactory.BuildCreateCaseDto(CaseWorker, trustSummaryDto.UkPrn);
			var postCaseDto = await PostCase(serviceScope, createCaseDto);

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
			using var serviceScope = _factory.Services.CreateScope();
			var caseService = serviceScope.ServiceProvider.GetRequiredService<ICaseService>();


			var trustSummaryDto = await FetchRandomTrust(serviceScope, "Senior");

			var createCaseDto = CaseFactory.BuildCreateCaseDto(CaseWorker, trustSummaryDto.UkPrn);
			var postCaseDto = await PostCase(serviceScope, createCaseDto);

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
				postCaseDto.StatusUrn,
				postCaseDto.RatingUrn);

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
			Assert.That(updatedCaseDto.RatingUrn, Is.EqualTo(postCaseDto.RatingUrn));
			Assert.That(updatedCaseDto.UpdatedAt, Is.EqualTo(timeNow));
			Assert.That(updatedCaseDto.DeEscalationPoint, Is.EqualTo(postCaseDto.DeEscalationPoint));
			Assert.That(updatedCaseDto.DirectionOfTravel, Is.EqualTo(DirectionOfTravelEnum.Improving.ToString()));
			Assert.That(updatedCaseDto.ReasonAtReview, Is.EqualTo("some updated reason at review"));
			Assert.That(updatedCaseDto.TrustUkPrn, Is.EqualTo(postCaseDto.TrustUkPrn));
		}
		
		private async Task<CaseDto> PostCase(IServiceScope serviceScope, CreateCaseDto createCaseDto)
		{
			var caseService = serviceScope.ServiceProvider.GetRequiredService<ICaseService>();
			return await caseService.PostCase(createCaseDto);
		}

		private async Task<TrustSearchDto> FetchRandomTrust(IServiceScope serviceScope, string searchParameter)
		{
			var trustService = serviceScope.ServiceProvider.GetRequiredService<ITrustService>();
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