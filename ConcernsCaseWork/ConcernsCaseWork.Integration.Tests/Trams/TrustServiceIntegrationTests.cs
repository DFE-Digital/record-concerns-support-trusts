using ConcernsCaseWork.Integration.Tests.Factory;
using ConcernsCaseWork.Services.Trusts;
using ConcernsCaseWork.Shared.Tests.Factory;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;
using Service.TRAMS.RecordAcademy;
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
			var trustService = _factory.Services.GetRequiredService<ITrustService>();
			const string searchParameter = "Northwood";
			
			// act
			var trustsPage = await trustService.GetTrustsByPagination(
				TrustFactory.CreateTrustSearch(searchParameter, searchParameter, searchParameter));

			// assert
			Assert.That(trustsPage, Is.Not.Null);
			Assert.That(trustsPage.Count, Is.GreaterThanOrEqualTo(1));
		}
		
		[Test]
		public async Task WhenRequestTrusts_ReturnsTrustsModelWithPagination()
		{
			// arrange
			var trustModelService = _factory.Services.GetRequiredService<ITrustModelService>();
			const string searchParameter = "Northwood";
			
			// act
			var trustsSummaryModel = await trustModelService.GetTrustsBySearchCriteria(
				TrustFactory.CreateTrustSearch(searchParameter, searchParameter, searchParameter));

			// assert
			Assert.That(trustsSummaryModel, Is.Not.Null);
			Assert.That(trustsSummaryModel.Count, Is.GreaterThanOrEqualTo(1));
		}
		
		[Test]
		public async Task WhenRequestTrustByUkPrn_ReturnsTrustDetailsDto()
		{
			// arrange
			var trustService = _factory.Services.GetRequiredService<ITrustService>();
			const string searchParameter = "Northwood";
			
			// act
			var trustsSummaryDto = await trustService.GetTrustsByPagination(TrustFactory.CreateTrustSearch(searchParameter, searchParameter, searchParameter));

			// assert
			Assert.That(trustsSummaryDto, Is.Not.Null);
			Assert.That(trustsSummaryDto.Count, Is.GreaterThanOrEqualTo(1));

			// arrange
			var ukPrn = trustsSummaryDto.Where(t => t.UkPrn != null).Select(t => t.UkPrn).FirstOrDefault();
			
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
			var trustModelService = _factory.Services.GetRequiredService<ITrustModelService>();
			const string searchParameter = "Northwood";
			
			// act
			var trustsSummaryModel = await trustModelService.GetTrustsBySearchCriteria(
				TrustFactory.CreateTrustSearch(searchParameter, searchParameter, searchParameter));

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