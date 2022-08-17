using ConcernsCasework.Service.Base;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Net.Mime;
using System.Text;

namespace ConcernsCasework.Service.RecordRatingHistory
{
	/// <summary>
	/// TODO waiting for PO, BA and DM to define if we will need this service at all or replace with a more generic one.
	/// </summary>
	public sealed class RecordRatingHistoryService : ConcernsAbstractService, IRecordRatingHistoryService
	{
		private readonly ILogger<RecordRatingHistoryService> _logger;
		
		public RecordRatingHistoryService(IHttpClientFactory clientFactory, ILogger<RecordRatingHistoryService> logger) : base(clientFactory)
		{
			_logger = logger;
		}

		public async Task<IList<RecordRatingHistoryDto>> GetRecordsRatingHistoryByCaseUrn(long caseUrn)
		{
			try
			{
				_logger.LogInformation("RecordRatingHistory::GetRecordsRatingHistoryByCaseUrn");
				
				// Create a request
				var request = new HttpRequestMessage(HttpMethod.Get, 
					$"/{EndpointsVersion}/record-rating-history/case/urn/{caseUrn}");
				
				// Create http client
				var client = ClientFactory.CreateClient(HttpClientName);
				
				// Execute request
				var response = await client.SendAsync(request);

				// Check status code
				response.EnsureSuccessStatusCode();
				
				// Read response content
				var content = await response.Content.ReadAsStringAsync();
				
				// Deserialize content to POJO
				var recordsRatingHistoryDto = JsonConvert.DeserializeObject<IList<RecordRatingHistoryDto>>(content);

				return recordsRatingHistoryDto;
			}
			catch (Exception ex)
			{
				_logger.LogError("RecordRatingHistory::GetRecordsRatingHistoryByCaseUrn::Exception message::{Message}", ex.Message);
			}
			
			return Array.Empty<RecordRatingHistoryDto>();
		}

		public async Task<IList<RecordRatingHistoryDto>> GetRecordsRatingHistoryByRecordUrn(long recordUrn)
		{
			try
			{
				_logger.LogInformation("RecordRatingHistory::GetRecordsRatingHistoryByRecordUrn");
				
				// Create a request
				var request = new HttpRequestMessage(HttpMethod.Get, 
					$"/{EndpointsVersion}/record-rating-history/record/urn/{recordUrn}");
				
				// Create http client
				var client = ClientFactory.CreateClient(HttpClientName);
				
				// Execute request
				var response = await client.SendAsync(request);

				// Check status code
				response.EnsureSuccessStatusCode();
				
				// Read response content
				var content = await response.Content.ReadAsStringAsync();
				
				// Deserialize content to POJO
				var recordsRatingHistoryDto = JsonConvert.DeserializeObject<IList<RecordRatingHistoryDto>>(content);

				return recordsRatingHistoryDto;
			}
			catch (Exception ex)
			{
				_logger.LogError("RecordRatingHistory::GetRecordsRatingHistoryByRecordUrn::Exception message::{Message}", ex.Message);
			}
			
			return Array.Empty<RecordRatingHistoryDto>();
		}

		public async Task<RecordRatingHistoryDto> PostRecordRatingHistory(RecordRatingHistoryDto recordRatingHistoryDto)
		{
			try
			{
				_logger.LogInformation("RecordRatingHistory::PostRecordRatingHistoryByRecordUrn");
				
				// Create a request
				var request = new StringContent(
					JsonConvert.SerializeObject(recordRatingHistoryDto),
					Encoding.UTF8,
					MediaTypeNames.Application.Json);
				
				// Create http client
				var client = ClientFactory.CreateClient(HttpClientName);
				
				// Execute request
				var response = await client.PostAsync($"/{EndpointsVersion}/record-rating-history", request);

				// Check status code
				response.EnsureSuccessStatusCode();
				
				// Read response content
				var content = await response.Content.ReadAsStringAsync();
				
				// Deserialize content to POJO
				var recordsRatingHistoryDto = JsonConvert.DeserializeObject<RecordRatingHistoryDto>(content);

				return recordsRatingHistoryDto;
			}
			catch (Exception ex)
			{
				_logger.LogError("RecordRatingHistory::PostRecordRatingHistoryByRecordUrn::Exception message::{Message}", ex.Message);
			}

			return null;
		}
	}
}