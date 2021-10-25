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
	public class AbstractIntegrationTests
	{
		internal static async Task Login(IConfigurationRoot configuration, HttpClient client)
		{
			// extract cookies for http client
			var login = await client.GetAsync("/login");
			Assert.That(login.StatusCode, Is.EqualTo(HttpStatusCode.OK));
			var foundCookie = login.Headers.TryGetValues("Set-Cookie", out IEnumerable<string> setCookie);
			
			// extract request verification token
			var htmlDoc = new HtmlDocument();
			htmlDoc.LoadHtml(await login.Content.ReadAsStringAsync());
			var tokenValue = htmlDoc.DocumentNode.SelectSingleNode("//input[@name='__RequestVerificationToken']")
				.Attributes.Where(att => att.Name == "value").Select(att => att.Value).First();
			
			// http client headers
			client.DefaultRequestHeaders.Clear();
			client.DefaultRequestHeaders.Add("Cookie", foundCookie ? setCookie : Enumerable.Empty<string>());
			
			var body = new FormUrlEncodedContent(new List<KeyValuePair<string, string>>
			{
				new KeyValuePair<string, string>("username", configuration["app:username"].Split(',').FirstOrDefault()),
				new KeyValuePair<string, string>("password", configuration["app:password"]),
				new KeyValuePair<string, string>("__RequestVerificationToken", tokenValue)
			});
			
			// Act
			var response = await client.PostAsync("/login", body);
			
			// Assert
			Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
			Assert.That(response.RequestMessage.RequestUri.AbsolutePath, Contains.Substring("/home"));
			Assert.That(await response.Content.ReadAsStringAsync(), Is.Not.Null);
		}
	}
}