using Ardalis.GuardClauses;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Service.TRAMS.Base;
using Service.TRAMS.Cases;
using Service.TRAMS.Records;
using System;
using System.Net.Http;
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

		public Task DeleteTeamCaseworkSelections(string ownerId)
		{
			throw new NotImplementedException();
		}

		public async Task<TeamCaseworkUsersSelectionDto> GetTeamCaseworkSelectedUsers(string ownerId)
		{
			// Create a request
			var request = new HttpRequestMessage(HttpMethod.Get,
				$"/{EndpointsVersion}/concerns-team-casework/owner/{ownerId}");

			// Create http client
			var client = ClientFactory.CreateClient(HttpClientName);

			// Execute request
			var response = await client.SendAsync(request);

			// Check status code
			response.EnsureSuccessStatusCode();

			// Read response content
			var content = await response.Content.ReadAsStringAsync();

			// Deserialize content to POCO
			var apiWrapperRecordsDto = JsonConvert.DeserializeObject<ApiWrapper<TeamCaseworkUsersSelectionDto>>(content);

			// Unwrap response
			if (apiWrapperRecordsDto is { Data: { } })
			{
				return apiWrapperRecordsDto.Data;
			}

			throw new Exception("Academies API error unwrap response");
		}

		public Task PutTeamCaseworkSelectedUsers(TeamCaseworkUsersSelectionDto selections)
		{
			throw new NotImplementedException();
		}
	}
}
