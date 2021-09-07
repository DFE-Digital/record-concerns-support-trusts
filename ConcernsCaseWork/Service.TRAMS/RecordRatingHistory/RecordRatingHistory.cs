using Microsoft.Extensions.Logging;
using Service.TRAMS.Base;
using Service.TRAMS.Models;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Mime;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Service.TRAMS.RecordRatingHistory
{
	public sealed class RecordRatingHistory : AbstractService, IRecordRatingHistory
	{
		private readonly ILogger<RecordRatingHistory> _logger;
		
		public RecordRatingHistory(IHttpClientFactory clientFactory, ILogger<RecordRatingHistory> logger) : base(clientFactory)
		{
			_logger = logger;
		}

		public async Task<IList<RecordRatingHistoryDto>> GetRecordsRatingHistoryByCaseUrn(string caseUrn)
		{
			try
			{
				_logger.LogInformation("RecordRatingHistory::GetRecordsRatingHistoryByCaseUrn");
				
				// Create a request
				var request = new HttpRequestMessage(HttpMethod.Get, $"/record-rating-history/case/urn/{caseUrn}");
				
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

		public async Task<IList<RecordRatingHistoryDto>> GetRecordsRatingHistoryByRecordUrn(string recordUrn)
		{
			try
			{
				_logger.LogInformation("RecordRatingHistory::GetRecordsRatingHistoryByRecordUrn");
				
				// Create a request
				var request = new HttpRequestMessage(HttpMethod.Get, $"/record-rating-history/record/urn/{recordUrn}");
				
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

		public async Task<RecordRatingHistoryDto> PostRecordRatingHistoryByRecordUrn(RecordRatingHistoryDto recordRatingHistoryDto)
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
				var response = await client.PostAsync("/record-rating-history", request);

				// Check status code
				response.EnsureSuccessStatusCode();
				
				// Read response content
				var content = await response.Content.ReadAsStringAsync();
				
				// Deserialize content to POJO
				var newRecordRatingHistoryDto = JsonSerializer.Deserialize<RecordRatingHistoryDto>(content, options);

				return newRecordRatingHistoryDto;
			}
			catch (Exception ex)
			{
				_logger.LogError($"RecordRatingHistory::PostRecordRatingHistoryByRecordUrn::Exception message::{ex.Message}");
			}
			
			return null;
		}
	}
}