using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Service.TRAMS.Base;
using Service.TRAMS.Sequence;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace Service.TRAMS.Status
{
	public sealed class StatusService : AbstractService, IStatusService
	{
		private readonly ILogger<StatusService> _logger;
		
		public StatusService(IHttpClientFactory clientFactory, ILogger<StatusService> logger) : base(clientFactory)
		{
			_logger = logger;
		}

		public async Task<IList<StatusDto>> GetStatuses()
		{
			try
			{
				_logger.LogInformation("StatusService::GetStatuses");
				
				// Create a request
				var request = new HttpRequestMessage(HttpMethod.Get, $"{EndpointsVersion}/statuses");
				
				// Create http client
				var client = ClientFactory.CreateClient("TramsClient");
				
				// Execute request
				var response = await client.SendAsync(request);

				// Check status code
				response.EnsureSuccessStatusCode();
				
				// Read response content
				var content = await response.Content.ReadAsStringAsync();
				
				// Deserialize content to POJO
				var statusDto = JsonConvert.DeserializeObject<IList<StatusDto>>(content);

				return statusDto;
			}
			catch (Exception ex)
			{
				_logger.LogError($"StatusService::GetStatuses::Exception message::{ex.Message}");
			}
			
			// TODO replace return when TRAMS API endpoints are live
			return new List<StatusDto>
			{
				new StatusDto("Live", DateTime.Now, DateTime.Now, LongSequence.Generator()),
				new StatusDto("Monitoring", DateTime.Now, DateTime.Now, LongSequence.Generator()),
				new StatusDto("Close", DateTime.Now, DateTime.Now, LongSequence.Generator())
			};

			//return Array.Empty<StatusDto>();
		}
	}
}