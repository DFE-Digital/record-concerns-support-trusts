using ConcernsCaseWork.Integration.Tests.Factory;
using ConcernsCaseWork.Shared.Tests.Factory;
using HtmlAgilityPack;
using Microsoft.Extensions.Configuration;
using NUnit.Framework;
using System.Net.Http;
using System.Threading.Tasks;

namespace ConcernsCaseWork.Integration.Tests.Pages
{
	[TestFixture]
	public class HomePageModelIntegrationTests : AbstractIntegrationTests
	{
		/// Testing the class requires a running Redis,
		/// startup is configured to use Redis with session storage.
		private IConfigurationRoot _configuration;
		private WebAppFactory _factory;
		private HttpClient _client;
		
		[OneTimeSetUp]
		public void OneTimeSetup()
		{
			_configuration = new ConfigurationBuilder().ConfigurationUserSecretsBuilder().Build();
			_factory = new WebAppFactory(_configuration);
			_client = _factory.CreateClient();
		}
		
		[OneTimeTearDown]
		public void OneTimeTearDown()
		{
			_client.Dispose();
			_factory.Dispose();
		}
		
		[Test]
		[Category("Smoke")]
		[Ignore("TODO: Revisit why pipeline is expecting 1")]
		public async Task WhenRequestingHomePage_ReturnsPage()
		{
			// arrange
			await Login(_configuration, _client);
			
			// act
			var logoutResponse = await _client.GetAsync("/home");
			
			var htmlDoc = new HtmlDocument();
			htmlDoc.LoadHtml(await logoutResponse.Content.ReadAsStringAsync());
			
			var logoutMessage = htmlDoc.DocumentNode.SelectNodes("//table");
			
			// assert
			Assert.That(logoutMessage, Is.Not.Null);
			Assert.That(logoutMessage.Count, Is.EqualTo(2));
		}
	}
}