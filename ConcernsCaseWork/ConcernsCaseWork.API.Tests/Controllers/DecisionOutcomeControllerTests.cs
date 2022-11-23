using ConcernsCaseWork.API.Contracts.Decisions.Outcomes;
using FluentAssertions;
using FluentAssertions.Common;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Mime;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace ConcernsCaseWork.API.Tests.Controllers
{
	public class DecisionOutcomeControllerTests : IDisposable
	{
		private WebApplicationFactory<Startup> _application;
		private HttpClient _client;

		public DecisionOutcomeControllerTests()
		{
			_application = new WebApplicationFactory<Startup>()
				.WithWebHostBuilder(builder =>
				{
					var configPath = Path.Combine(Directory.GetCurrentDirectory(), "appsettings.tests.json");

					builder.ConfigureAppConfiguration((context, config) =>
					{
						config.AddJsonFile(configPath);
					});
				});

			_client = _application.CreateClient();
			_client.DefaultRequestHeaders.Add("ConcernsApiKey", "app-key");
		}

		public void Dispose()
		{
			_application.Dispose();
			_client.Dispose();
		}

		[Fact]
		public async Task When_DecisionOutcomeCreated_Returns_201Response()
		{
			var request = new CreateDecisionOutcomeRequest();
			var body = JsonConvert.SerializeObject(request);

			var result = await _client.PostAsync($"/v2/concerns-cases/1/decisions/1/outcome", CreateJsonPayload(body));

			result.StatusCode.Should().Be(HttpStatusCode.Created);
		}

		[Fact]
		public async Task When_DecisionOutcomeUpdated_Returns_201Response()
		{
			var request = new CreateDecisionOutcomeRequest();
			var body = JsonConvert.SerializeObject(request);

			var result = await _client.PutAsync($"/v2/concerns-cases/1/decisions/1/outcome", CreateJsonPayload(body));

			result.StatusCode.Should().Be(HttpStatusCode.OK);
		}

		private StringContent CreateJsonPayload(string body)
		{
			return new StringContent(body, Encoding.UTF8, MediaTypeNames.Application.Json);
		}
	}
}
