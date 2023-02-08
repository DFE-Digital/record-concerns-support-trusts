using ConcernsCaseWork.Integration.Tests.Factory;
using ConcernsCaseWork.Service.Trusts;
using ConcernsCaseWork.Services.Trusts;
using ConcernsCaseWork.Shared.Tests.Factory;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;
using System.Linq;
using System.Threading.Tasks;

namespace ConcernsCaseWork.Integration.Tests.Trams
{
	[TestFixture]
	public class TrustServiceIntegrationTests
	{
		/// <summary>
		/// Testing the class requires a running Redis,
		/// startup is configured to use Redis with session storage.
		/// </summary>
		private IConfigurationRoot _configuration;
		private WebAppFactory _factory;

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
		public async Task WhenRequestTrusts_ReturnsTrustsDtoWithPagination()
		{
			// arrange
			using var serviceScope = _factory.Services.CreateScope();
			var trustService = serviceScope.ServiceProvider.GetRequiredService<ITrustService>();
			const string searchParameter = "Northwood";

			// act
			var apiWrapperTrustsSummaryDto = await trustService.GetTrustsByPagination(
				TrustFactory.BuildTrustSearch(searchParameter, searchParameter, searchParameter), 1);

			// assert
			Assert.That(apiWrapperTrustsSummaryDto, Is.Not.Null);
			Assert.That(apiWrapperTrustsSummaryDto.Trusts, Is.Not.Null);
			Assert.That(apiWrapperTrustsSummaryDto.Trusts.Count, Is.GreaterThanOrEqualTo(1));
		}

		[Test]
		public async Task WhenRequestTrusts_ReturnsTrustsModelWithPagination()
		{
			// arrange
			using var serviceScope = _factory.Services.CreateScope();
			var trustModelService = serviceScope.ServiceProvider.GetRequiredService<ITrustModelService>();
			const string searchParameter = "Northwood";

			// act
			var trustsSummaryModel = await trustModelService.GetTrustsBySearchCriteria(
				TrustFactory.BuildTrustSearch(searchParameter, searchParameter, searchParameter));

			// assert
			Assert.That(trustsSummaryModel, Is.Not.Null);
			Assert.That(trustsSummaryModel.Count, Is.GreaterThanOrEqualTo(1));
		}

		[Test]
		public async Task WhenRequestTrustByUkPrn_ReturnsTrustDetailsDto()
		{
			// arrange
			using var serviceScope = _factory.Services.CreateScope();
			var trustService = serviceScope.ServiceProvider.GetRequiredService<ITrustService>();
			const string searchParameter = "Northwood";

			// act
			var apiWrapperTrustsSummaryDto = await trustService.GetTrustsByPagination(TrustFactory.BuildTrustSearch(searchParameter, searchParameter, searchParameter), 1);

			// assert
			Assert.That(apiWrapperTrustsSummaryDto, Is.Not.Null);
			Assert.That(apiWrapperTrustsSummaryDto.Trusts, Is.Not.Null);
			Assert.That(apiWrapperTrustsSummaryDto.Trusts.Count, Is.GreaterThanOrEqualTo(1));

			// arrange
			var ukPrn = apiWrapperTrustsSummaryDto.Trusts.Where(t => t.UkPrn != null).Select(t => t.UkPrn).FirstOrDefault();

			// act
			var trustDetailsDto = await trustService.GetTrustByUkPrn(ukPrn);

			// assert
			Assert.That(trustDetailsDto, Is.Not.Null);
			Assert.That(trustDetailsDto.GiasData, Is.Not.Null);
			Assert.That(trustDetailsDto.GiasData.GroupContactAddress, Is.Not.Null);
		}

		[Test]
		public async Task WhenRequestTrustByUkPrn_ReturnsTrustDetailsModel()
		{
			// arrange
			using var serviceScope = _factory.Services.CreateScope();
			var trustModelService = serviceScope.ServiceProvider.GetRequiredService<ITrustModelService>();
			const string searchParameter = "Northwood";

			// act
			var trustsSummaryModel = await trustModelService.GetTrustsBySearchCriteria(
				TrustFactory.BuildTrustSearch(searchParameter, searchParameter, searchParameter));

			// assert
			Assert.That(trustsSummaryModel, Is.Not.Null);
			Assert.That(trustsSummaryModel.Count, Is.GreaterThanOrEqualTo(1));

			// arrange
			var ukPrn = trustsSummaryModel.Where(t => t.UkPrn != null).Select(t => t.UkPrn).FirstOrDefault();

			// act
			var trustDetailsModel = await trustModelService.GetTrustByUkPrn(ukPrn);

			// assert
			Assert.That(trustDetailsModel, Is.Not.Null);
			Assert.That(trustDetailsModel.GiasData, Is.Not.Null);
			Assert.That(trustDetailsModel.GiasData.GroupContactAddress, Is.Not.Null);
		}
	}
}