using Microsoft.Extensions.Logging;
using Service.TRAMS.Base;
using Service.TRAMS.Models;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Numerics;
using System.Text.Json;
using System.Threading.Tasks;

namespace Service.TRAMS.Cases
{
	public sealed class CaseService : AbstractService, ICaseService
	{
		private readonly ILogger<CaseService> _logger;
		
		public CaseService(IHttpClientFactory clientFactory, ILogger<CaseService> logger) : base(clientFactory)
		{
			_logger = logger;
		}
		
		public async Task<IList<CaseDto>> GetCasesByCaseworker(string caseworker)
		{
			try
			{
				_logger.LogInformation("CaseService::GetCasesByCaseworker");
				
				// Create a request
				var request = new HttpRequestMessage(HttpMethod.Get,
					"https://api.github.com/repos/dotnet/AspNetCore.Docs/branches");
				
				// Create http client
				var client = ClientFactory.CreateClient("TramsClient");
				
				// Execute request
				var response = await client.SendAsync(request);

				// Check status code
				response.EnsureSuccessStatusCode();
				
				// Read response content
				var content = await response.Content.ReadAsStringAsync();
				
				// Deserialize content to POJO
				var options = new JsonSerializerOptions(JsonSerializerDefaults.Web);
				var cases = JsonSerializer.Deserialize<IList<CaseDto>>(content, options);

				return cases;
			}
			catch (Exception ex)
			{
				_logger.LogError($"CaseService::GetCasesByCaseworker::Exception message::{ex.Message}");
			}

			// Uncomment when pointing service to real trams api
			//return Array.Empty<CaseDto>();
			
			var dateTimeNow = DateTimeOffset.Now;
			return new List<CaseDto>
			{
				new CaseDto(
					1, dateTimeNow, dateTimeNow, dateTimeNow, dateTimeNow, "testing","description", "crmenquiry",
					"trustukprn", "reasonatreview", dateTimeNow, "issue", "currentstatus", "nextSteps", 
					"resolutionstrategy", "directionoftravel", BigInteger.One, Int32.MinValue
				),
				new CaseDto(
					2, dateTimeNow, dateTimeNow, dateTimeNow, dateTimeNow, "testing","description", "crmenquiry",
					"trustukprn", "reasonatreview", dateTimeNow, "issue", "currentstatus", "nextSteps", 
					"resolutionstrategy", "directionoftravel", BigInteger.One, Int32.MinValue
				),
				new CaseDto(
					3, dateTimeNow, dateTimeNow, dateTimeNow, dateTimeNow, "testing","description", "crmenquiry",
					"trustukprn", "reasonatreview", dateTimeNow, "issue", "currentstatus", "nextSteps", 
					"resolutionstrategy", "directionoftravel", BigInteger.One, Int32.MinValue
				),
				new CaseDto(
					4, dateTimeNow, dateTimeNow, dateTimeNow, dateTimeNow, "testing","description", "crmenquiry",
					"trustukprn", "reasonatreview", dateTimeNow, "issue", "currentstatus", "nextSteps", 
					"resolutionstrategy", "directionoftravel", BigInteger.One, Int32.MinValue
				),
				new CaseDto(
					5, dateTimeNow, dateTimeNow, dateTimeNow, dateTimeNow, "testing","description", "crmenquiry",
					"trustukprn", "reasonatreview", dateTimeNow, "issue", "currentstatus", "nextSteps", 
					"resolutionstrategy", "directionoftravel", BigInteger.One, Int32.MinValue
				)
			};
		}
	}
}