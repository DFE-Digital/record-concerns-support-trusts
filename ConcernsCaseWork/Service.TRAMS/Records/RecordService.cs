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

namespace Service.TRAMS.Status
{
	public sealed class RecordService : AbstractService, IRecordService
	{
		private readonly ILogger<RecordService> _logger;
		
		public RecordService(IHttpClientFactory clientFactory, ILogger<RecordService> logger) : base(clientFactory)
		{
			_logger = logger;
		}
		
		public async Task<IList<RecordDto>> GetRecordsByCaseUrn(BigInteger caseUrn)
		{
			try
			{
				_logger.LogInformation("RecordService::GetRecordsByCaseUrn");
				
				// Create a request
				var request = new HttpRequestMessage(HttpMethod.Get, 
					$"{EndpointsVersion}/records/case/urn/{caseUrn}");
				
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
				var recordsDto = JsonSerializer.Deserialize<IList<RecordDto>>(content, options);

				return recordsDto;
			}
			catch (Exception ex)
			{
				_logger.LogError($"RecordService::GetRecordsByCaseUrn::Exception message::{ex.Message}");
			}
			
			return Array.Empty<RecordDto>();
		}

		public async Task<RecordDto> PostRecordByCaseUrn(CreateRecordDto createRecordDto)
		{
			try
			{
				_logger.LogInformation("RecordService::PostRecordByCaseUrn");
				
				// Create a request
				var options = new JsonSerializerOptions(JsonSerializerDefaults.Web);
				var request = new StringContent(
					JsonSerializer.Serialize(createRecordDto, options),
					Encoding.UTF8,
					MediaTypeNames.Application.Json);
				
				// Create http client
				var client = ClientFactory.CreateClient("TramsClient");
				
				// Execute request
				var response = await client.PostAsync(
					$"{EndpointsVersion}/record/case/urn/{createRecordDto.CaseUrn}", request);

				// Check status code
				response.EnsureSuccessStatusCode();
				
				// Read response content
				var content = await response.Content.ReadAsStringAsync();
				
				// Deserialize content to POJO
				var newRecordDto = JsonSerializer.Deserialize<RecordDto>(content, options);

				return newRecordDto;
			}
			catch (Exception ex)
			{
				_logger.LogError($"RecordService::PostRecordByCaseUrn::Exception message::{ex.Message}");
			}
			
			return null;
		}

		public async Task<RecordDto> PatchRecordByUrn(UpdateRecordDto updateRecordDto)
		{
			try
			{
				_logger.LogInformation("RecordService::PatchRecordByUrn");
				
				// Create a request
				var options = new JsonSerializerOptions(JsonSerializerDefaults.Web);
				var request = new StringContent(
					JsonSerializer.Serialize(updateRecordDto, options),
					Encoding.UTF8,
					MediaTypeNames.Application.Json);

				// Create http client
				var client = ClientFactory.CreateClient("TramsClient");
				
				// Execute request
				var response = await client.PatchAsync(
					$"{EndpointsVersion}/record/urn/{updateRecordDto.Urn}", request);

				// Check status code
				response.EnsureSuccessStatusCode();
				
				// Read response content
				var content = await response.Content.ReadAsStringAsync();
				
				// Deserialize content to POJO
				var updatedRecordDto = JsonSerializer.Deserialize<RecordDto>(content, options);

				return updatedRecordDto;
			}
			catch (Exception ex)
			{
				_logger.LogError($"RecordService::PatchRecordByUrn::Exception message::{ex.Message}");
			}

			return null;
		}
	}
}