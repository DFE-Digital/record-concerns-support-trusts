using ConcernsCaseWork.Integration.Tests.Factory;
using ConcernsCaseWork.Shared.Tests.Factory;
using HtmlAgilityPack;
using Microsoft.Extensions.Configuration;
using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace ConcernsCaseWork.Integration.Tests.Pages
{
	[TestFixture]
	public class LoginPageModelIntegrationTests : AbstractIntegrationTests
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
		[Ignore("Not valid test for authentication with Azure AD")]
		public async Task WhenSignInIsWithCorrectCredentials_ReturnHomePage()
		{
			// arrange
			await Login(_configuration, _client);
			
			// Logout
			var logout = await _client.GetAsync("/logout");
			Assert.That(logout.StatusCode, Is.EqualTo(HttpStatusCode.OK));
		}
		
		[Test]
		[Category("Smoke")]
		[Ignore("Not valid test for authentication with Azure AD")]
		public async Task WhenSignInIsWithInCorrectCredentials_ReturnLoginPage()
		{
			// arrange
			
			// extract cookies for http client
			var login = await _client.GetAsync("/login");
			Assert.That(login.StatusCode, Is.EqualTo(HttpStatusCode.OK));
			var foundCookie = login.Headers.TryGetValues("Set-Cookie", out IEnumerable<string> setCookie);
			
			// extract request verification token
			var htmlDoc = new HtmlDocument();
			htmlDoc.LoadHtml(await login.Content.ReadAsStringAsync());
			var tokenValue = htmlDoc.DocumentNode.SelectSingleNode("//input[@name='__RequestVerificationToken']")
				.Attributes.Where(att => att.Name == "value").Select(att => att.Value).First();
			
			// http client headers
			_client.DefaultRequestHeaders.Clear();
			_client.DefaultRequestHeaders.Add("Cookie", foundCookie ? setCookie : Enumerable.Empty<string>());
			
			var body = new FormUrlEncodedContent(new List<KeyValuePair<string, string>>
			{
				new KeyValuePair<string, string>("username", "test"),
				new KeyValuePair<string, string>("password", "test"),
				new KeyValuePair<string, string>("__RequestVerificationToken", tokenValue)
			});
			
			// Act
			var response = await _client.PostAsync("/login", body);
			
			// Assert
			Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
			Assert.That(response.RequestMessage.RequestUri.AbsolutePath, Contains.Substring("/login"));
			Assert.That(await response.Content.ReadAsStringAsync(), Is.Not.Null);
			
			// Logout
			var logout = await _client.GetAsync("/logout");
			Assert.That(logout.StatusCode, Is.EqualTo(HttpStatusCode.OK));
		}
	}
}