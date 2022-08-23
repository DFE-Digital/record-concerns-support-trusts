using Ardalis.GuardClauses;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Service.TRAMS.Base;
using System;
using System.Net.Http;
using System.Net.Mime;
using System.Text;
using System.Threading.Tasks;

namespace Service.TRAMS.Teams
{
	public class TeamsService : AbstractService, ITeamsService
	{
		private readonly ILogger<TeamsService> _logger;

		public TeamsService(IHttpClientFactory clientFactory, ILogger<TeamsService> logger) : base(clientFactory)
		{
			_logger = Guard.Against.Null(logger);
		}

		public async Task<ConcernsCaseworkTeamDto> GetTeamCaseworkSelectedUsers(string ownerId)
		{
			Guard.Against.NullOrWhiteSpace(ownerId);

			// Create a request
			var request = new HttpRequestMessage(HttpMethod.Get,
				$"/{EndpointsVersion}/concerns-team-casework/owner/{ownerId}");

			// Create http client
			var client = ClientFactory.CreateClient(HttpClientName);

			// Execute request
			var response = await client.SendAsync(request);

			// Check status code
			response.EnsureSuccessStatusCode();

			if (response.StatusCode == System.Net.HttpStatusCode.NoContent)
			{
				return null;
			}

			// Read response content
			var content = await response.Content.ReadAsStringAsync();

			// Deserialize content to POCO
			var apiWrapperRecordsDto = JsonConvert.DeserializeObject<ApiWrapper<ConcernsCaseworkTeamDto>>(content);

			// Unwrap response
			if (apiWrapperRecordsDto is { Data: { } })
			{
				return apiWrapperRecordsDto.Data;
			}

			throw new Exception("Academies API error unwrap response");
		}

		public async Task PutTeamCaseworkSelectedUsers(ConcernsCaseworkTeamDto team)
		{
			Guard.Against.Null(team);

			// Create a request
			var request = new StringContent(
				JsonConvert.SerializeObject(team),
				Encoding.UTF8,
				MediaTypeNames.Application.Json);

			// Create http client
			var client = ClientFactory.CreateClient(HttpClientName);

			// Execute request
			var uri = $"/{EndpointsVersion}/concerns-team-casework/owner/{team.OwnerId}";
			var response = await client.PutAsync(uri, request);

			// Check status code
			response.EnsureSuccessStatusCode();

			// Read response content
			var content = await response.Content.ReadAsStringAsync();

			// Deserialize content to POCO
			var apiWrapperRecordsDto = JsonConvert.DeserializeObject<ApiWrapper<ConcernsCaseworkTeamDto>>(content);

			// Unwrap response
			if (apiWrapperRecordsDto is { Data: { } })
			{
				return;
			}
			throw new Exception("Academies API error unwrap response");
		}
	}
}
