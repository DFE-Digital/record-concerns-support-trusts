using ConcernsCaseWork.Integration.Tests.Factory;
using ConcernsCaseWork.Service.Trusts;
using ConcernsCaseWork.Shared.Tests.Factory;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;
using System.Threading.Tasks;

namespace ConcernsCaseWork.Integration.Tests.Trams
{
	[TestFixture]
	public class TrustSearchServiceIntegrationTests
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
		public async Task WhenGetTrustsCachedWithSearchByGroupName_ReturnsTrustsFromTrams()
		{
			// arrange
			using var serviceScope = _factory.Services.CreateScope();
			var trustService = serviceScope.ServiceProvider.GetRequiredService<ITrustSearchService>();
			const string searchParameter = "Northwood";

			// act
			var results = await trustService.GetTrustsBySearchCriteria(TrustFactory.BuildTrustSearch(searchParameter));

			// assert
			Assert.That(results, Is.Not.Null);
			Assert.That(results.Trusts.Count, Is.GreaterThanOrEqualTo(1));
		}

		[Test]
		public async Task WhenGetTrustsCachedWithSearchByGroupNameAndUkPrn_ReturnsTrustsFromTrams()
		{
			// arrange
			using var serviceScope = _factory.Services.CreateScope();
			var trustService = serviceScope.ServiceProvider.GetRequiredService<ITrustSearchService>();
			const string searchParameter = "Northwood";

			// act
			var results = await trustService.GetTrustsBySearchCriteria(TrustFactory.BuildTrustSearch(searchParameter, searchParameter));

			// assert
			Assert.That(results, Is.Not.Null);
			Assert.That(results.Trusts.Count, Is.GreaterThanOrEqualTo(1));
		}

		[Test]
		public async Task WhenGetTrustsCachedWithSearchByGroupNameAndUkPrnAndCompaniesHouseNumber_ReturnsTrustsFromTrams()
		{
			// arrange
			using var serviceScope = _factory.Services.CreateScope();
			var trustService = serviceScope.ServiceProvider.GetRequiredService<ITrustSearchService>();
			const string searchParameter = "Northwood";

			// act
			var results = await trustService.GetTrustsBySearchCriteria(TrustFactory.BuildTrustSearch(searchParameter, searchParameter, searchParameter));

			// assert
			Assert.That(results, Is.Not.Null);
			Assert.That(results.Trusts.Count, Is.GreaterThanOrEqualTo(1));
		}
	}
}