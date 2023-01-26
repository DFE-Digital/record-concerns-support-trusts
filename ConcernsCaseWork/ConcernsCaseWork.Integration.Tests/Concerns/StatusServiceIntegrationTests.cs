using ConcernsCaseWork.Integration.Tests.Factory;
using ConcernsCaseWork.Integration.Tests.Helpers;
using ConcernsCaseWork.Service.Status;
using ConcernsCaseWork.Shared.Tests.Factory;
using ConcernsCaseWork.UserContext;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;
using System.Threading.Tasks;

namespace ConcernsCaseWork.Integration.Tests.Concerns
{
	[TestFixture]
	public class StatusServiceIntegrationTests
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
		public async Task WhenGetStatuses_ReturnsStatuses()
		{
			// arrange
			using var serviceScope = _factory.Services.CreateScope();
			serviceScope.ServiceProvider.GetService<IClientUserInfoService>().SetPrincipal(ClaimsPrincipalTestHelper.CreateCaseWorkerPrincipal());
			var statusService = serviceScope.ServiceProvider.GetRequiredService<IStatusService>();
			
			// act
			var statusesDto = await statusService.GetStatuses();

			// assert
			Assert.That(statusesDto, Is.Not.Null);
			Assert.That(statusesDto.Count, Is.EqualTo(3));
		}
	}
}