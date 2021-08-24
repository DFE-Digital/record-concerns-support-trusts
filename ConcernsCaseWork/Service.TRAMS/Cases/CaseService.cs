using Service.TRAMS.Base;
using Service.TRAMS.Models;
using System.Collections.Generic;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

namespace Service.TRAMS.Cases
{
	public sealed class CaseService : AbstractService, ICaseService
	{
		public CaseService(IHttpClientFactory clientFactory) : base(clientFactory)
		{
			
		}
		
		public async Task<IEnumerable<CaseDto>> GetCasesByCaseworker(string caseworker)
		{
			// Create a request
			var request = new HttpRequestMessage(HttpMethod.Get,
				"https://api.github.com/repos/dotnet/AspNetCore.Docs/branches");
			
			// Create http client
			var client = ClientFactory.CreateClient("TramsClient");
			
			// Execute request
			var response = await client.SendAsync(request);

			if (!response.IsSuccessStatusCode)
			{
				return new List<CaseDto>
				{
					new CaseDto("CI-1004634", "-", "Wintermute Academy Trust", 0, 1),
					new CaseDto("CI-1004635", "Safeguarding", "Straylight Academies", 1, 3),
					new CaseDto("CI-1004636", "Finance", "The Linda Lee Academies Trust", 2, 12),
					new CaseDto("CI-1004637", "Governance", "Wintermute Academy Trust", 3, 17),
					new CaseDto("CI-1004638", "Finance", "Armitage Education Trust", 4, 32)
				};
				
				// return Array.Empty<CaseDto>();
			}
			
			var content = await response.Content.ReadAsStringAsync();
			return JsonSerializer.Deserialize<IEnumerable<CaseDto>>(content);
		}
	}
}