using Microsoft.Extensions.Logging;
using Service.TRAMS.Base;
using Service.TRAMS.Models;
using System;
using System.Collections.Generic;
using System.Net.Http;
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
				_logger.LogError($"TrustService::GetTrustsByPagination::Exception message::{ex.Message}");
			}

			// Uncomment when pointing service to real trams api
			//return Array.Empty<CaseDto>();
			
			return new List<CaseDto>
			{
				new CaseDto("CI-1004634", "-", "Wintermute Academy Trust", 0, "09-08-2021", "09-08-2021", "09-08-2021"),
				new CaseDto("CI-1004635", "Safeguarding", "Straylight Academies", 1, "09-08-2021", "09-08-2021", "09-08-2021"),
				new CaseDto("CI-1004636", "Finance", "The Linda Lee Academies Trust", 2, "09-08-2021", "09-08-2021", "09-08-2021"),
				new CaseDto("CI-1004637", "Governance", "Wintermute Academy Trust", 3, "09-08-2021", "09-08-2021", "09-08-2021"),
				new CaseDto("CI-1004638", "Finance", "Armitage Education Trust", 4, "09-08-2021", "09-08-2021", "09-08-2021")
			};
		}
	}
}