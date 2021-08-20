using ConcernsCaseWork.Tests.Factory;
using HtmlAgilityPack;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace ConcernsCaseWork.Tests.Pages
{
	[TestFixture]
	public class LoginModelIntegrationTests
	{
		/// Testing the class requires a running Redis,
		/// because startup is configured to use Redis with session storage.
		private WebAppFactory _factory;
		private HttpClient _client;
		
		[OneTimeSetUp]
		public void OneTimeSetup()
		{
			_factory = new WebAppFactory(ConfigurationFactory.LoginIntegrationConfigurationBuilder());
			_client = _factory.CreateClient();
		}
		
		[OneTimeTearDown]
		public void OneTimeTearDown()
		{
			_client.Dispose();
			_factory.Dispose();
		}
		
		[Test]
		public async Task WhenSignInIsWithCorrectCredentials_ReturnHomePage()
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
				new KeyValuePair<string, string>("username", "username"),
				new KeyValuePair<string, string>("password", "password"),
				new KeyValuePair<string, string>("__RequestVerificationToken", tokenValue)
			});
			
			// Act
			var response = await _client.PostAsync("/login", body);
			
			// Assert
			Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
			Assert.That(response.RequestMessage.RequestUri.AbsolutePath, Contains.Substring("/home"));
			Assert.That(await response.Content.ReadAsStringAsync(), Is.Not.Null);
			
			// Logout
			var logout = await _client.GetAsync("/logout");
			Assert.That(logout.StatusCode, Is.EqualTo(HttpStatusCode.OK));
		}
		
		[Test]
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