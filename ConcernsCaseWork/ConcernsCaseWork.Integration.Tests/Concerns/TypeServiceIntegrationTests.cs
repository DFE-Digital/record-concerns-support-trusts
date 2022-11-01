using ConcernsCaseWork.Integration.Tests.Factory;
using ConcernsCaseWork.Service.Types;
using ConcernsCaseWork.Shared.Tests.Factory;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;
using System.Threading.Tasks;

namespace ConcernsCaseWork.Integration.Tests.Concerns
{
	[TestFixture]
	public class TypeServiceIntegrationTests
	{
		/// Testing the class requires a running Redis,
		/// startup is configured to use Redis with session storage.
		private IConfigurationRoot _configuration;
		private WebAppFactory _factory;

		/// <summary>
		/// Variables for caseworker and trustukprn, creates cases on Academies API.
		/// Future work can be to delete the records from the SQLServer.
		/// </summary>
		
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
		public async Task WhenGetTypes_ReturnsListTypeDto()
		{
			// arrange
			using var serviceScope = _factory.Services.CreateScope();
			var typeService = serviceScope.ServiceProvider.GetRequiredService<ITypeService>();

			//act 
			var typesDto = await typeService.GetTypes();

			// assert
			Assert.That(typesDto, Is.Not.Null);
			Assert.That(typesDto.Count, Is.EqualTo(13));
		}
	}
}