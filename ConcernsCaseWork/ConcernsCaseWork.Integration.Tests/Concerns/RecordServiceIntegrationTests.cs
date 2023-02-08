using ConcernsCaseWork.Integration.Tests.Factory;
using ConcernsCaseWork.Integration.Tests.Helpers;
using ConcernsCaseWork.Service.Cases;
using ConcernsCaseWork.Service.MeansOfReferral;
using ConcernsCaseWork.Service.Ratings;
using ConcernsCaseWork.Service.Records;
using ConcernsCaseWork.Service.Trusts;
using ConcernsCaseWork.Service.Types;
using ConcernsCaseWork.Shared.Tests.Factory;
using ConcernsCaseWork.UserContext;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace ConcernsCaseWork.Integration.Tests.Concerns
{
	[TestFixture]
	public class RecordServiceIntegrationTests
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
		public async Task WhenCreateAndGetRecordsByCaseUrn_ReturnsListRecordDto()
		{
			// arrange
			using var serviceScope = _factory.Services.CreateScope();
			serviceScope.ServiceProvider.GetService<IClientUserInfoService>().SetPrincipal(ClaimsPrincipalTestHelper.CreateCaseWorkerPrincipal());
			var startTime = DateTime.UtcNow;
			var recordService = serviceScope.ServiceProvider.GetRequiredService<IRecordService>();
			var caseUrn = await FetchRandomCaseUrn(serviceScope);
			var typeId = await FetchRandomTypeId(serviceScope);
			var ratingId = await FetchRandomRatingId(serviceScope);
			var meansOfReferralId = await FetchRandomMeansOfReferralId(serviceScope);
			var createRecordDto = RecordFactory.BuildCreateRecordDto(caseUrn, typeId, ratingId, meansOfReferralId);
			await PostRecord(serviceScope, createRecordDto);

			//act 
			var recordsDto = await recordService.GetRecordsByCaseUrn(caseUrn);

			// Assert
			var endTime = DateTime.UtcNow;
			
			Assert.That(recordsDto, Is.Not.Null);
			Assert.That(recordsDto.Count, Is.EqualTo(1));
			
			var resultDto = recordsDto.ToList().Single();
			Assert.AreEqual(caseUrn, resultDto.CaseUrn);
			Assert.AreEqual(typeId, resultDto.TypeId);
			Assert.AreEqual(ratingId, resultDto.RatingId);
			Assert.NotNull(resultDto.Description);
			Assert.NotNull(resultDto.Name);
			Assert.NotNull(resultDto.Reason);
			Assert.Greater(resultDto.Id, 0);
			Assert.That(resultDto.CreatedAt >= startTime && resultDto.CreatedAt <= endTime);
			Assert.That(resultDto.UpdatedAt >= startTime && resultDto.UpdatedAt <= endTime);
		}

		[Test]
		public async Task WhenPatchRecordByUrn_UpdatesRecord()
		{
			// arrange 
			using var serviceScope = _factory.Services.CreateScope();
			serviceScope.ServiceProvider.GetService<IClientUserInfoService>().SetPrincipal(ClaimsPrincipalTestHelper.CreateCaseWorkerPrincipal());
			var recordService = serviceScope.ServiceProvider.GetRequiredService<IRecordService>();
			var caseUrn = await FetchRandomCaseUrn(serviceScope);
			var typeId = await FetchRandomTypeId(serviceScope);
			var ratingId = await FetchRandomRatingId(serviceScope);
			var updatedRatingId = await FetchRandomRatingId(serviceScope);
			var createRecordDto = RecordFactory.BuildCreateRecordDto(caseUrn, typeId, ratingId);
			var postRecordDto = await PostRecord(serviceScope, createRecordDto);

			// Update record properties
			var timeNow = DateTimeOffset.Now;
			var toUpdateRecordDto = new RecordDto(
				postRecordDto.CreatedAt,
				timeNow,
				timeNow,
				timeNow,
				postRecordDto.Name,
				"some updated description",
				"some updated reason",
				postRecordDto.CaseUrn,
				postRecordDto.TypeId,
				updatedRatingId,
				postRecordDto.Id,
				postRecordDto.StatusId
				);

			// act 
			var updatedRecordDto = await recordService.PatchRecordById(toUpdateRecordDto);

			// assert
			Assert.That(updatedRecordDto, Is.Not.Null);
			Assert.That(updatedRecordDto.CreatedAt, Is.EqualTo(postRecordDto.CreatedAt));
			Assert.That(updatedRecordDto.UpdatedAt, Is.EqualTo(timeNow));
			Assert.That(updatedRecordDto.ReviewAt, Is.EqualTo(timeNow));
			Assert.That(updatedRecordDto.ClosedAt, Is.EqualTo(timeNow));
			Assert.That(updatedRecordDto.Name, Is.EqualTo(postRecordDto.Name));
			Assert.That(updatedRecordDto.Description, Is.EqualTo("some updated description"));
			Assert.That(updatedRecordDto.Reason, Is.EqualTo("some updated reason"));
			Assert.That(updatedRecordDto.CaseUrn, Is.EqualTo(postRecordDto.CaseUrn));
			Assert.That(updatedRecordDto.TypeId, Is.EqualTo(postRecordDto.TypeId));
			Assert.That(updatedRecordDto.RatingId, Is.EqualTo(updatedRatingId));
			Assert.That(updatedRecordDto.Id, Is.EqualTo(postRecordDto.Id));
			Assert.That(updatedRecordDto.StatusId, Is.EqualTo(postRecordDto.StatusId));
		}

		private async Task<RecordDto> PostRecord(IServiceScope serviceScope, CreateRecordDto createRecordDto)
		{
			var recordService = serviceScope.ServiceProvider.GetRequiredService<IRecordService>();
			return await recordService.PostRecordByCaseUrn(createRecordDto);
		}

		private async Task<string> FetchRandomTrustUkprn(IServiceScope serviceScope)
		{
			const string searchParameter = "Senior";
			var trustService = serviceScope.ServiceProvider.GetRequiredService<ITrustService>();
			var apiWrapperTrusts = await trustService.GetTrustsByPagination(
				TrustFactory.BuildTrustSearch(searchParameter, searchParameter, searchParameter), 1);

			Assert.That(apiWrapperTrusts, Is.Not.Null);
			Assert.That(apiWrapperTrusts.Trusts, Is.Not.Null);

			var random = new Random();
			int index = random.Next(apiWrapperTrusts.Trusts.Count);

			return apiWrapperTrusts.Trusts[index].UkPrn;
		}
		
		private async Task<long> FetchRandomCaseUrn(IServiceScope serviceScope)
		{
			var caseService = serviceScope.ServiceProvider.GetRequiredService<ICaseService>();
			var trustUkprn = await FetchRandomTrustUkprn(serviceScope);
			var createCaseDto = CaseFactory.BuildCreateCaseDto(CaseWorker, trustUkprn);
			var caseDto = await caseService.PostCase(createCaseDto);

			return caseDto.Urn;
		}

		private async Task<long> FetchRandomTypeId(IServiceScope serviceScope)
		{
			var typeService = serviceScope.ServiceProvider.GetRequiredService<ITypeService>();
			var types = await typeService.GetTypes();

			var random = new Random();
			var index = random.Next(types.Count);

			return types[index].Id;
		}

		private async Task<long> FetchRandomRatingId(IServiceScope serviceScope)
		{
			var ratingService = serviceScope.ServiceProvider.GetRequiredService<IRatingService>();
			var ratings = await ratingService.GetRatings();

			var random = new Random();
			var index = random.Next(ratings.Count);

			return ratings[index].Id;
		}
		
		private async Task<long> FetchRandomMeansOfReferralId(IServiceScope serviceScope)
		{
			var meansOfReferralService = serviceScope.ServiceProvider.GetRequiredService<IMeansOfReferralService>();
			var meansOfReferrals = await meansOfReferralService.GetMeansOfReferrals();

			var random = new Random();
			var index = random.Next(meansOfReferrals.Count);

			return meansOfReferrals[index].Id;
		}
	}
}