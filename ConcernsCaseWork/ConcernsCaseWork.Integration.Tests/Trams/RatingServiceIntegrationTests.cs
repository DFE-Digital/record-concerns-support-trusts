using ConcernsCaseWork.Integration.Tests.Factory;
using ConcernsCaseWork.Service.Ratings;
using ConcernsCaseWork.Shared.Tests.Factory;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;
using System.Threading.Tasks;

namespace ConcernsCaseWork.Integration.Tests.Trams
{
	[TestFixture]
	public class RatingServiceIntegrationTests
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
		public async Task WhenGetRatings_ReturnsListRatingDto()
		{
			// arrange
			using var serviceScope = _factory.Services.CreateScope();
			var ratingService = serviceScope.ServiceProvider.GetRequiredService<IRatingService>();

			//act 
			var ratingsDto = await ratingService.GetRatings();

			// assert
			Assert.That(ratingsDto, Is.Not.Null);
			Assert.That(ratingsDto.Count, Is.EqualTo(5));
		}
	}
}