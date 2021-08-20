using ConcernsCaseWork.Tests.Factory;
using HtmlAgilityPack;
using Microsoft.Extensions.Configuration;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace ConcernsCaseWork.Tests.Factory
{
	[TestFixture]
	public class LoginModelIntegrationTests
	{
		private WebAppFactory _factory;
		private HttpClient _client;
		
		[OneTimeSetUp]
		public void OneTimeSetup()
		{
			_factory = new WebAppFactory(SetupConfiguration());
			_client = _factory.CreateClient();
		}

		private static IConfigurationRoot SetupConfiguration()
		{
			var configuration = new Dictionary<string, string> { { "username", "username" }, { "password", "password" } };
			return new ConfigurationBuilder().AddInMemoryCollection(configuration).Build();
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
				new KeyValuePair<string, string>("username", Environment.GetEnvironmentVariable("username")),
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
			Assert.That(response.RequestMessage.RequestUri.AbsolutePath, Contains.Substring("/login"));
			Assert.That(await response.Content.ReadAsStringAsync(), Is.Not.Null);
			
			// Logout
			var logout = await _client.GetAsync("/logout");
			Assert.That(logout.StatusCode, Is.EqualTo(HttpStatusCode.OK));
		}
	}
}