using ConcernsCaseWork.Integration.Tests.Factory;
using ConcernsCaseWork.Shared.Tests.Factory;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;
using Service.TRAMS.Trusts;
using System.Threading.Tasks;

namespace ConcernsCaseWork.Integration.Tests.Trams
{
	[TestFixture]
	public class TrustSearchServiceIntegrationTests
	{
		/// Testing the class requires a running Redis,
		/// startup is configured to use Redis with session storage.
		private IConfigurationRoot _configuration;
		private WebAppFactory _factory;
		
		[OneTimeSetUp]
		public void OneTimeSetup()
		{
			_configuration = ConfigurationFactory.ConfigurationUserSecretsBuilder();
			_factory = new WebAppFactory(_configuration);
		}
		
		[OneTimeTearDown]
		public void OneTimeTearDown()
		{
			_factory.Dispose();
		}

		[Test]
		public async Task WhenGetTrustsCached_ReturnsTrustsFromTrams()
		{
			// arrange
			var trustService = _factory.Services.GetRequiredService<ITrustSearchService>();
			const string searchParameter = "Northwood";
			
			// act
			var trusts = await trustService.GetTrustsBySearchCriteria(TrustSearchFactory.CreateTrustSearch(searchParameter));

			// assert
			Assert.That(trusts, Is.Not.Null);
			Assert.That(trusts.Count, Is.GreaterThanOrEqualTo(1));
		}
	}
}