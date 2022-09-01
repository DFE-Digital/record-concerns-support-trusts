using ConcernsCaseWork.Integration.Tests.Factory;
using ConcernsCaseWork.Shared.Tests.Factory;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;
using Service.TRAMS.Cases;
using Service.TRAMS.MeansOfReferral;
using Service.TRAMS.Ratings;
using Service.TRAMS.Records;
using Service.TRAMS.Trusts;
using Service.TRAMS.Types;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace ConcernsCaseWork.Integration.Tests.Trams
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
			var startTime = DateTime.UtcNow;
			var recordService = serviceScope.ServiceProvider.GetRequiredService<IRecordService>();
			var caseUrn = await FetchRandomCaseUrn(serviceScope);
			var typeUrn = await FetchRandomTypeUrn(serviceScope);
			var ratingUrn = await FetchRandomRatingUrn(serviceScope);
			var meansOfReferralUrn = await FetchRandomMeansOfReferralUrn(serviceScope);
			var createRecordDto = RecordFactory.BuildCreateRecordDto(caseUrn, typeUrn, ratingUrn, meansOfReferralUrn);
			await PostRecord(serviceScope, createRecordDto);

			//act 
			var recordsDto = await recordService.GetRecordsByCaseUrn(caseUrn);

			// Assert
			var endTime = DateTime.UtcNow;
			
			Assert.That(recordsDto, Is.Not.Null);
			Assert.That(recordsDto.Count, Is.EqualTo(1));
			
			var resultDto = recordsDto.ToList().Single();
			Assert.AreEqual(caseUrn, resultDto.CaseUrn);
			Assert.AreEqual(typeUrn, resultDto.TypeUrn);
			Assert.AreEqual(ratingUrn, resultDto.RatingUrn);
			Assert.NotNull(resultDto.Description);
			Assert.NotNull(resultDto.Name);
			Assert.NotNull(resultDto.Reason);
			Assert.Greater(resultDto.Urn, 0);
			Assert.That(resultDto.ClosedAt >= startTime && resultDto.ClosedAt <= endTime);
			Assert.That(resultDto.CreatedAt >= startTime && resultDto.CreatedAt <= endTime);
			Assert.That(resultDto.UpdatedAt >= startTime && resultDto.UpdatedAt <= endTime);
		}

		[Test]
		public async Task WhenPatchRecordByUrn_UpdatesRecord()
		{
			// arrange 
			using var serviceScope = _factory.Services.CreateScope();
			var recordService = serviceScope.ServiceProvider.GetRequiredService<IRecordService>();
			var caseUrn = await FetchRandomCaseUrn(serviceScope);
			var typeUrn = await FetchRandomTypeUrn(serviceScope);
			var ratingUrn = await FetchRandomRatingUrn(serviceScope);
			var updatedRatingUrn = await FetchRandomRatingUrn(serviceScope);
			var createRecordDto = RecordFactory.BuildCreateRecordDto(caseUrn, typeUrn, ratingUrn);
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
				postRecordDto.TypeUrn,
				updatedRatingUrn,
				postRecordDto.Urn,
				postRecordDto.StatusUrn
				);

			// act 
			var updatedRecordDto = await recordService.PatchRecordByUrn(toUpdateRecordDto);

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
			Assert.That(updatedRecordDto.TypeUrn, Is.EqualTo(postRecordDto.TypeUrn));
			Assert.That(updatedRecordDto.RatingUrn, Is.EqualTo(updatedRatingUrn));
			Assert.That(updatedRecordDto.Urn, Is.EqualTo(postRecordDto.Urn));
			Assert.That(updatedRecordDto.StatusUrn, Is.EqualTo(postRecordDto.StatusUrn));
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
				TrustFactory.BuildTrustSearch(searchParameter, searchParameter, searchParameter));

			Assert.That(apiWrapperTrusts, Is.Not.Null);
			Assert.That(apiWrapperTrusts.Data, Is.Not.Null);

			var random = new Random();
			int index = random.Next(apiWrapperTrusts.Data.Count);

			return apiWrapperTrusts.Data[index].UkPrn;
		}
		
		private async Task<long> FetchRandomCaseUrn(IServiceScope serviceScope)
		{
			var caseService = serviceScope.ServiceProvider.GetRequiredService<ICaseService>();
			var trustUkprn = await FetchRandomTrustUkprn(serviceScope);
			var createCaseDto = CaseFactory.BuildCreateCaseDto(CaseWorker, trustUkprn);
			var caseDto = await caseService.PostCase(createCaseDto);

			return caseDto.Urn;
		}

		private async Task<long> FetchRandomTypeUrn(IServiceScope serviceScope)
		{
			var typeService = serviceScope.ServiceProvider.GetRequiredService<ITypeService>();
			var types = await typeService.GetTypes();

			var random = new Random();
			var index = random.Next(types.Count);

			return types[index].Urn;
		}

		private async Task<long> FetchRandomRatingUrn(IServiceScope serviceScope)
		{
			var ratingService = serviceScope.ServiceProvider.GetRequiredService<IRatingService>();
			var ratings = await ratingService.GetRatings();

			var random = new Random();
			var index = random.Next(ratings.Count);

			return ratings[index].Urn;
		}
		
		private async Task<long> FetchRandomMeansOfReferralUrn(IServiceScope serviceScope)
		{
			var meansOfReferralService = serviceScope.ServiceProvider.GetRequiredService<IMeansOfReferralService>();
			var meansOfReferrals = await meansOfReferralService.GetMeansOfReferrals();

			var random = new Random();
			var index = random.Next(meansOfReferrals.Count);

			return meansOfReferrals[index].Urn;
		}
	}
}