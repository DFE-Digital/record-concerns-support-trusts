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
		
		public RecordService(IHttpClientFactory clientFactory, ILogger<RecordService> logger) : base(clientFactory, logger)
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
					$"/{EndpointsVersion}/concerns-records/case/urn/{caseUrn}");
				
				// Create http client
				var client = ClientFactory.CreateClient(HttpClientName);
				
				// Execute request
				var response = await client.SendAsync(request);

				// Check status code
				response.EnsureSuccessStatusCode();
				
				// Read response content
				var content = await response.Content.ReadAsStringAsync();
				
				// Deserialize content to POCO
				var apiWrapperRecordsDto = JsonConvert.DeserializeObject<ApiListWrapper<RecordDto>>(content);

				// Unwrap response
				if (apiWrapperRecordsDto is { Data: { } })
				{
					return apiWrapperRecordsDto.Data;
				}

				throw new Exception("Academies API error unwrap response");
			}
			catch (Exception ex)
			{
				_logger.LogError("RecordService::GetRecordsByCaseUrn::Exception message::{Message}", ex.Message);
				
				throw;
			}
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
				var client = ClientFactory.CreateClient(HttpClientName);
				
				// Execute request
				var response = await client.PostAsync(
					$"/{EndpointsVersion}/concerns-records", request);

				// Check status code
				response.EnsureSuccessStatusCode();
				
				// Read response content
				var content = await response.Content.ReadAsStringAsync();
				
				// Deserialize content to POCO
				var apiWrapperRecordDto = JsonConvert.DeserializeObject<ApiWrapper<RecordDto>>(content);

				// Unwrap response
				if (apiWrapperRecordDto is { Data: { } })
				{
					return apiWrapperRecordDto.Data;
				}

				throw new Exception("Academies API error unwrap response");
			}
			catch (Exception ex)
			{
				_logger.LogError("RecordService::PostRecordByCaseUrn::Exception message::{Message}", ex.Message);
				
				throw;
			}
		}

		public async Task<RecordDto> PatchRecordById(RecordDto recordDto)
		{
			try
			{
				_logger.LogInformation("RecordService::PatchRecordById");
				
				// Create a request
				var request = new StringContent(
					JsonConvert.SerializeObject(recordDto),
					Encoding.UTF8,
					MediaTypeNames.Application.Json);

				// Create http client
				var client = ClientFactory.CreateClient(HttpClientName);
				
				// Execute request
				var response = await client.PatchAsync(
					$"/{EndpointsVersion}/concerns-records/{recordDto.Id}", request);

				// Check status code
				response.EnsureSuccessStatusCode();
				
				// Read response content
				var content = await response.Content.ReadAsStringAsync();
				
				// Deserialize content to POCO
				var apiWrapperRecordDto = JsonConvert.DeserializeObject<ApiWrapper<RecordDto>>(content);

				// Unwrap response
				if (apiWrapperRecordDto is { Data: { } })
				{
					return apiWrapperRecordDto.Data;
				}

				throw new Exception("Academies API error unwrap response");
			}
			catch (Exception ex)
			{
				_logger.LogError("RecordService::PatchRecordByUrn::Exception message::{Message}", ex.Message);

				throw;
			}
		}
	}
}