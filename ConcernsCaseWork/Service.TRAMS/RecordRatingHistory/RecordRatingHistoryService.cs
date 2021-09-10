using Microsoft.Extensions.Logging;
using Service.TRAMS.Base;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Mime;
using System.Numerics;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Service.TRAMS.RecordRatingHistory
{
	public sealed class RecordRatingHistoryService : AbstractService, IRecordRatingHistoryService
	{
		private readonly ILogger<RecordRatingHistoryService> _logger;
		
		public RecordRatingHistoryService(IHttpClientFactory clientFactory, ILogger<RecordRatingHistoryService> logger) : base(clientFactory)
		{
			_logger = logger;
		}

		public async Task<IList<RecordRatingHistoryDto>> GetRecordsRatingHistoryByCaseUrn(BigInteger caseUrn)
		{
			try
			{
				_logger.LogInformation("RecordRatingHistory::GetRecordsRatingHistoryByCaseUrn");
				
				// Create a request
				var request = new HttpRequestMessage(HttpMethod.Get, 
					$"{EndpointsVersion}/record-rating-history/case/urn/{caseUrn}");
				
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
				var recordsRatingHistoryDto = JsonSerializer.Deserialize<IList<RecordRatingHistoryDto>>(content, options);

				return recordsRatingHistoryDto;
			}
			catch (Exception ex)
			{
				_logger.LogError($"RecordRatingHistory::GetRecordsRatingHistoryByCaseUrn::Exception message::{ex.Message}");
			}
			
			return Array.Empty<RecordRatingHistoryDto>();
		}

		public async Task<IList<RecordRatingHistoryDto>> GetRecordsRatingHistoryByRecordUrn(BigInteger recordUrn)
		{
			try
			{
				_logger.LogInformation("RecordRatingHistory::GetRecordsRatingHistoryByRecordUrn");
				
				// Create a request
				var request = new HttpRequestMessage(HttpMethod.Get, 
					$"{EndpointsVersion}/record-rating-history/record/urn/{recordUrn}");
				
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
				var recordsRatingHistoryDto = JsonSerializer.Deserialize<IList<RecordRatingHistoryDto>>(content, options);

				return recordsRatingHistoryDto;
			}
			catch (Exception ex)
			{
				_logger.LogError($"RecordRatingHistory::GetRecordsRatingHistoryByRecordUrn::Exception message::{ex.Message}");
			}
			
			return Array.Empty<RecordRatingHistoryDto>();
		}

		public async Task<RecordRatingHistoryDto> PostRecordRatingHistory(RecordRatingHistoryDto recordRatingHistoryDto)
		{
			try
			{
				_logger.LogInformation("RecordRatingHistory::PostRecordRatingHistoryByRecordUrn");
				
				// Create a request
				var options = new JsonSerializerOptions(JsonSerializerDefaults.Web);
				var request = new StringContent(
					JsonSerializer.Serialize(recordRatingHistoryDto, options),
					Encoding.UTF8,
					MediaTypeNames.Application.Json);
				
				// Create http client
				var client = ClientFactory.CreateClient("TramsClient");
				
				// Execute request
				var response = await client.PostAsync($"{EndpointsVersion}/record-rating-history", request);

				// Check status code
				response.EnsureSuccessStatusCode();
				
				// Read response content
				var content = await response.Content.ReadAsStringAsync();
				
				// Deserialize content to POJO
				var recordsRatingHistoryDto = JsonSerializer.Deserialize<RecordRatingHistoryDto>(content, options);

				return recordsRatingHistoryDto;
			}
			catch (Exception ex)
			{
				_logger.LogError($"RecordRatingHistory::PostRecordRatingHistoryByRecordUrn::Exception message::{ex.Message}");
			}

			return null;
		}
	}
}