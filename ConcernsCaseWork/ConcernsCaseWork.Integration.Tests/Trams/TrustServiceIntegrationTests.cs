using ConcernsCaseWork.Integration.Tests.Factory;
using ConcernsCaseWork.Shared.Tests.Factory;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;
using Service.TRAMS.Trusts;
using System.Linq;
using System.Threading.Tasks;

namespace ConcernsCaseWork.Integration.Tests.Trams
{
	[TestFixture]
	public class TrustServiceIntegrationTests
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
		public async Task WhenRequestTrusts_ReturnsTrustsWithPagination()
		{
			// arrange
			var trustService = _factory.Services.GetRequiredService<ITrustService>();

			// act
			var trustsPage = await trustService.GetTrustsByPagination();

			// assert
			Assert.That(trustsPage, Is.Not.Null);
			Assert.That(trustsPage.Count(), Is.EqualTo(10));
		}
	}
}