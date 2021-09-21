using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Service.TRAMS.Base;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Mime;
using System.Text;
using System.Threading.Tasks;

namespace Service.TRAMS.Records
{
	public sealed class RecordService : AbstractService, IRecordService
	{
		private readonly ILogger<RecordService> _logger;
		
		public RecordService(IHttpClientFactory clientFactory, ILogger<RecordService> logger) : base(clientFactory)
		{
			_logger = logger;
		}
		
		public async Task<IList<RecordDto>> GetRecordsByCaseUrn(long caseUrn)
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
				var recordsDto = JsonConvert.DeserializeObject<IList<RecordDto>>(content);

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
				var request = new StringContent(
					JsonConvert.SerializeObject(createRecordDto),
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
				var newRecordDto = JsonConvert.DeserializeObject<RecordDto>(content);

				return newRecordDto;
			}
			catch (Exception ex)
			{
				_logger.LogError($"RecordService::PostRecordByCaseUrn::Exception message::{ex.Message}");
				
				throw;
			}
		}

		public async Task<RecordDto> PatchRecordByUrn(RecordDto recordDto)
		{
			try
			{
				_logger.LogInformation("RecordService::PatchRecordByUrn");
				
				// Create a request
				var request = new StringContent(
					JsonConvert.SerializeObject(recordDto),
					Encoding.UTF8,
					MediaTypeNames.Application.Json);

				// Create http client
				var client = ClientFactory.CreateClient("TramsClient");
				
				// Execute request
				var response = await client.PatchAsync(
					$"{EndpointsVersion}/record/urn/{recordDto.Urn}", request);

				// Check status code
				response.EnsureSuccessStatusCode();
				
				// Read response content
				var content = await response.Content.ReadAsStringAsync();
				
				// Deserialize content to POJO
				var updatedRecordDto = JsonConvert.DeserializeObject<RecordDto>(content);

				return updatedRecordDto;
			}
			catch (Exception ex)
			{
				_logger.LogError($"RecordService::PatchRecordByUrn::Exception message::{ex.Message}");

				throw;
			}
		}
	}
}